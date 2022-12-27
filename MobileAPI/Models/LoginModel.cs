using System.ComponentModel.DataAnnotations;
using Utils.Models;

namespace MobileAPI.Models
{
    public class LoginModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Username { get; set; }
    }

    public class LoginEVNModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string EVNCode { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Fullname { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string Username { get; set; }

        public string Address { get; set; }
    }

    public class DeviceModel : DeviceTokenModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string DeviceId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public OSType OS { get; set; }

        public bool IsGetNotice { get; set; }
    }

    public class DeviceTokenModel
    {
        public string Token { get; set; }
    }
}
