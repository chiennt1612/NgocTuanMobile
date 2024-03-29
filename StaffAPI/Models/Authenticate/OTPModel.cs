﻿using System.ComponentModel.DataAnnotations;

namespace StaffAPI.Models.Authenticate
{
    public class OTPModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Code { get; set; }
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Username { get; set; }
        public string DeviceId { get; set; }
    }
}
