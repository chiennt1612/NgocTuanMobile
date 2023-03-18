using System.ComponentModel.DataAnnotations;

namespace StaffAPI.Models
{
    public class CustomerModel
    {
        [StringLength(20, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(EntityFramework.API.Resources.EntityValidation))]
        public string CustomerCode { get; set; }
    }
}
