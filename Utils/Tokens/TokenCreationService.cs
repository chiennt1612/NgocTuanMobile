using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utils.Tokens.Extensions;
using Utils.Tokens.Interfaces;

namespace Utils.Tokens
{
    public class TokenCreationService : ITokenCreationService
    {
        protected readonly ILogger Logger;
        protected readonly ISystemClock Clock;
        private readonly IConfiguration Configuration;
        public TokenCreationService(
            ISystemClock clock,
            ILogger<TokenCreationService> logger,
            IConfiguration configuration)
        {
            Clock = clock;
            Logger = logger;
            Configuration = configuration;
        }

        public virtual async Task<string> CreateTokenAsync(Token token)
        {
            var header = CreateHeaderAsync(token);
            var payload = await CreatePayloadAsync(token);

            return await CreateJwtAsync(new JwtSecurityToken(header, payload));
        }

        protected virtual JwtHeader CreateHeaderAsync(Token token)
        {
            var credential = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"])), SecurityAlgorithms.HmacSha256);// Certificate.Get();
            var header = new JwtHeader(credential);

            header["typ"] = "at+jwt";

            return header;
        }

        protected virtual Task<JwtPayload> CreatePayloadAsync(Token token)
        {
            var payload = token.CreateJwtPayload(Clock, Logger);
            return Task.FromResult(payload);
        }

        protected virtual Task<string> CreateJwtAsync(JwtSecurityToken jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            return Task.FromResult(handler.WriteToken(jwt));
        }

        public virtual Token CreateAccessTokenAsync(IList<Claim> Claims)
        {
            Logger.LogTrace("Creating access token");

            Claims.Add(new Claim(JwtClaimTypes.JwtId, CryptoRandom.CreateUniqueId(16, CryptoRandom.OutputFormat.Hex)));

            // iat claim as required by JWT profile
            Claims.Add(new Claim(JwtClaimTypes.IssuedAt, Clock.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64));

            var token = new Token(OidcConstants.TokenTypes.AccessToken)
            {
                CreationTime = Clock.UtcNow.UtcDateTime,
                Issuer = Configuration["JWT:ValidIssuer"],
                Lifetime = int.Parse(Configuration["JWT:TokenValidityInMinutes"]) * 60,
                Claims = Claims,
                ClientId = Guid.NewGuid().ToString(),
                Description = "",
                AccessTokenType = AccessTokenType.Jwt,
                AllowedSigningAlgorithms = new List<string>()
            };

            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public bool ValidateToken(string? token)
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,

                ValidAudience = Configuration["JWT:ValidAudience"],
                ValidIssuer = Configuration["JWT:ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token.Substring("Bearer ".Length), tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || principal == null)
                    return false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                return false;
            }

            return true;

        }
    }
}
