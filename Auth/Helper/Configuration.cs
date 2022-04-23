using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Helper
{
    public class RegisterConfiguration
    {
        public bool Enabled { get; set; } = true;
    }

    public enum LoginResolutionPolicy
    {
        Username = 0,
        Email = 1
    }
    public class LoginConfiguration
    {
        public LoginResolutionPolicy ResolutionPolicy { get; set; } = LoginResolutionPolicy.Username;
    }
}
