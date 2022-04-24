using System.ComponentModel.DataAnnotations;

namespace Auth.Models
{
    public class RefreshTokenModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string? RefreshToken { get; set; }
    }
}
