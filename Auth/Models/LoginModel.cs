using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class LoginModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string? Username { get; set; }
    }
}
