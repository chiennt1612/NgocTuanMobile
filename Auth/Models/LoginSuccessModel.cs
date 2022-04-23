using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils.Models;

namespace Auth.Models
{
    public class LoginSuccessModel : ResponseBase
    {
        public LoginData data { get; set; }
    }

    public class LoginData
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
