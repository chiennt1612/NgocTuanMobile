using System.Collections.Generic;
using System.Security.Claims;

namespace Utils.Tokens.Interfaces
{
    public interface IClaimsService
    {
        IEnumerable<Claim> GetAccessTokenClaimsAsync(ClaimsPrincipal subject);
    }
}
