using Utils.Models;

namespace StaffAPI.Models
{
    public class LoginSuccessModel : ResponseBase
    {
        public LoginData data { get; set; }
    }

    public class LoginData
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
