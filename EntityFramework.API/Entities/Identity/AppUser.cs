using Microsoft.AspNetCore.Identity;
using System;

namespace EntityFramework.API.Entities.Identity
{
    public class AppUser : IdentityUser<long>
    {
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
