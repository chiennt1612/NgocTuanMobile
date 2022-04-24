using EntityFramework.API.Entities.EntityBase;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.API.Entities
{
    public class FAQ : MetaEntity
    {
        [StringLength(300, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Title", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Title { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Summary", ResourceType = typeof(Resources.EntityValidation))]
        public string Summary { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Description", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Description { get; set; }

        [Display(Name = "Publisher", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Publisher { get; set; }
    }
}
