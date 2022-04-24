using IdentityModel;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Utils.Tokens.Extensions;
using Utils.Tokens.Interfaces;

namespace Utils.Tokens
{

    public class ClaimsService : IClaimsService
    {
        protected readonly ILogger Logger;
        public ClaimsService(ILogger<ClaimsService> logger)
        {
            Logger = logger;
        }

        protected virtual IEnumerable<Claim> GetStandardSubjectClaims(ClaimsPrincipal subject)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, subject.GetSubjectId()),
                new Claim(JwtClaimTypes.AuthenticationTime, subject.GetAuthenticationTimeEpoch().ToString(), ClaimValueTypes.Integer64),
            };

            claims.AddRange(subject.GetAuthenticationMethods());

            return claims;
        }

        public virtual IEnumerable<Claim> GetAccessTokenClaimsAsync(ClaimsPrincipal subject)
        {
            var outputClaims = new List<Claim>
            {
                new Claim(JwtClaimTypes.ClientId, Guid.NewGuid().ToString())
            };

            // a user is involved
            if (subject != null)
            {
                Logger.LogDebug("Getting claims for access token for subject: {subject}", subject.GetSubjectId());

                outputClaims.AddRange(GetStandardSubjectClaims(subject));
            }

            return outputClaims;
        }
    }
}
