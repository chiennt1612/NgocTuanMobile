using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Utils.Tokens.Interfaces
{
    public interface ITokenCreationService
    {
        Task<string> CreateTokenAsync(Token token);
        Token CreateAccessTokenAsync(IList<Claim> Claims);
        string GenerateRefreshToken();
        bool ValidateToken(string? token);
    }
}
