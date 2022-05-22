using EntityFramework.API.Entities.EntityBase;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.API.Entities
{
    public class Categories : MetaEntity
    {
        //public Categories()
        //{
        //    Code = Guid.NewGuid().ToString();
        //}
        public ICollection<Product> Products { get; set; }
        public ICollection<Product> ReferProducts { get; set; }
        // For Company
        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "CateName", ResourceType = typeof(Resources.EntityValidation))]
        public string Name { get; set; }

        //[Display(Name = "CateCode", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[StringLength(30, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //public string Code { get; set; }

        [StringLength(200)]
        [Display(Name = "CategoryImage", ResourceType = typeof(Resources.EntityValidation))]
        public string Img { get; set; }

        [Display(Name = "DisplayOnHome", ResourceType = typeof(Resources.EntityValidation))]// home
        public int DisplayOnHome { get; set; }

        [ForeignKey("Categories")]
        [Display(Name = "CateParent", ResourceType = typeof(Resources.EntityValidation))]
        public long? ParentId { get; set; }
        [Display(Name = "CateParent", ResourceType = typeof(Resources.EntityValidation))]
        public Categories Parent { get; set; }

        [Display(Name = "DisplayOnMenuLeft", ResourceType = typeof(Resources.EntityValidation))] // Sales-off
        public bool Status { get; set; }

        [Display(Name = "DisplayOnMenuMain", ResourceType = typeof(Resources.EntityValidation))] // mobile
        public bool DisplayOnMenuMain { get; set; }
    }

    public class NewsCategories : MetaEntity
    {
        //public NewsCategories()
        //{
        //    //Code = Guid.NewGuid().ToString();
        //}
        public ICollection<Article> Articles { get; set; }
        public ICollection<Article> ReferArticles { get; set; }
        // For Company
        [StringLength(200, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "CateName", ResourceType = typeof(Resources.EntityValidation))]
        public string Name { get; set; }

        //[Display(Name = "CateCode", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[StringLength(30, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //public string Code { get; set; }

        [StringLength(200)]
        [Display(Name = "CategoryImage", ResourceType = typeof(Resources.EntityValidation))]
        public string Img { get; set; }

        [Display(Name = "DisplayOnHome", ResourceType = typeof(Resources.EntityValidation))]
        public bool DisplayOnHome { get; set; }

        [ForeignKey("NewsCategories")]
        [Display(Name = "CateParent", ResourceType = typeof(Resources.EntityValidation))]
        public long? ParentId { get; set; }
        [Display(Name = "CateParent", ResourceType = typeof(Resources.EntityValidation))]
        public NewsCategories Parent { get; set; }

        [Display(Name = "DisplayOnMenuLeft", ResourceType = typeof(Resources.EntityValidation))]
        public bool Status { get; set; }

        [Display(Name = "DisplayOnMenuMain", ResourceType = typeof(Resources.EntityValidation))]
        public bool DisplayOnMenuMain { get; set; }
    }
}
