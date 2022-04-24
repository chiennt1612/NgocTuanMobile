using EntityFramework.API.Entities.EntityBase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.API.Entities
{
    public class Service : MetaEntity
    {
        [StringLength(300, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Title", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Title { get; set; }

        [StringLength(200)]
        [Display(Name = "ServiceImage", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Img { get; set; }

        [StringLength(2000)]
        [Display(Name = "Summary", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Summary { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "GroupIdList", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string GroupIdList { get; set; }

        [Display(Name = "Price1", ResourceType = typeof(Resources.EntityValidation))]
        public double Price { get; set; }
        [Display(Name = "Price2", ResourceType = typeof(Resources.EntityValidation))]
        public double Price1 { get; set; }
        [StringLength(2000)]
        [Display(Name = "PriceText", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string PriceText { get; set; }
        public ICollection<Contact> Contacts { get; set; }
    }
}
