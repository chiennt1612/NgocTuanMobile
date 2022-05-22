using EntityFramework.API.Entities.EntityBase;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.API.Entities
{
    public class ParamSetting : BaseEntity
    {
        [StringLength(30, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "ParamKey", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string ParamKey { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Language", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Language { get; set; }

        [StringLength(500, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "ParamValue", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string ParamValue { get; set; }


        //// For Company
        //[StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[Display(Name = "CompanyLogo", ResourceType = typeof(Resources.EntityValidation))]
        //public string Logo { get; set; }

        //[Display(Name = "CompanySince", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[StringLength(100, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //public string Since { get; set; }

        //[Display(Name = "CompanyName", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //public string Name { get; set; }

        //[Display(Name = "Address", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //public string Address { get; set; }

        //[Display(Name = "Hotline", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[StringLength(50, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //public string Hotline { get; set; }

        //[Display(Name = "Phone", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[StringLength(50, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //public string Phone { get; set; }

        //[Display(Name = "Email", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //public string Email { get; set; }

    }
}
