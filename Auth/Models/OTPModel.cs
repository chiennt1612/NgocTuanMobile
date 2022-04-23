using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class OTPModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string? OTP { get; set; }
    }
}
