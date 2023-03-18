using System.ComponentModel.DataAnnotations;

namespace StaffAPI.Models.Authenticate
{
    public class RefreshTokenModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string RefreshToken { get; set; }
        public string DeviceId { get; set; }
    }
}
