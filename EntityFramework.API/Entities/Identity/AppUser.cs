using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using Utils.Models;

namespace EntityFramework.API.Entities.Identity
{
    public class AppUser : IdentityUser<long>
    {
        public int TotalOTP { get; set; }
        public DateTime? OTPSendTime { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }

    public class AppUserDevice
    {
        public long Id { get; set; }
        [StringLength(30)]
        public string Username { get; set; }
        [StringLength(36)]
        public string DeviceID { get; set; }
        public string Token { get; set; }
        public OSType OS { get; set; }
        public bool IsGetNotice { get; set; } = false;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
