using System.ComponentModel.DataAnnotations;

namespace EntityFramework.API.Entities.EntityBase
{
    public class MetaEntity : BaseEntity
    {
        [Display(Name = "MetaTitle", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(2000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string MetaTitle { get; set; }

        [Display(Name = "MetaDescription", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(2000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string MetaDescription { get; set; }

        [Display(Name = "MetaKeyword", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(2000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string MetaKeyword { get; set; }

        [Display(Name = "MetaBox", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(2000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string MetaBox { get; set; }

        [Display(Name = "MetaRobotTag", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(2000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string MetaRobotTag { get; set; }
    }
}
