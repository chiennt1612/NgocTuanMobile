using Microsoft.IdentityModel.Tokens;

namespace Utils.Tokens.Models
{
    public class SecurityKeyInfo
    {
        public SecurityKey Key { get; set; }
        public string SigningAlgorithm { get; set; }
    }
}
