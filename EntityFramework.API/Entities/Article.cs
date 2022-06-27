//using System.Web.Mvc;
using EntityFramework.API.Entities.EntityBase;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EntityFramework.API.Entities
{
    public class Article : MetaEntity
    {
        [ForeignKey("MainCategories")]
        [Display(Name = "CateMain", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public long CategoryMain { get; set; }
        [JsonIgnore]
        [Display(Name = "NewsCategories", ResourceType = typeof(Resources.EntityValidation))]
        public NewsCategories MainCategories { get; set; }

        [ForeignKey("ReferCategories")]
        //[StringLength(1000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "CateRefer", ResourceType = typeof(Resources.EntityValidation))]
        public long? CategoryReference { get; set; }
        [JsonIgnore]
        public NewsCategories ReferCategories { get; set; }
        // For Company
        //[StringLength(500, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "TagsRefer", ResourceType = typeof(Resources.EntityValidation))]
        public string TagsReference { get; set; }

        // For Company
        [StringLength(300, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Title", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Title { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Img", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Img { get; set; }

        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "ImgBanner", ResourceType = typeof(Resources.EntityValidation))]
        public string ImgBanner { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Summary", ResourceType = typeof(Resources.EntityValidation))]
        public string Summary { get; set; }


        [DataType(DataType.MultilineText)]
        [Display(Name = "Description", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Description { get; set; }

        [Display(Name = "IsNews", ResourceType = typeof(Resources.EntityValidation))]
        public bool IsNews { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Resources.EntityValidation))]
        public bool Status { get; set; }

        [Display(Name = "Publisher", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Publisher { get; set; }
    }
}
