using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.API.Entities
{
    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(128, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Fullname", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Fullname { get; set; }

        [StringLength(20, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[Display(Name = "Mobile", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Mobile { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Address", ResourceType = typeof(Resources.EntityValidation))]
        public string Address { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Email", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [EmailAddress(ErrorMessageResourceName = "EmailIsNotValid", ErrorMessageResourceType = typeof(LanguageAll.Language))]
        public string Email { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "CompanyName", ResourceType = typeof(Resources.EntityValidation))]
        public string CompanyName { get; set; } // CMND

        public bool IsCompany { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Description", ResourceType = typeof(Resources.EntityValidation))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Description { get; set; }

        [ForeignKey("Services")]
        [Display(Name = "ServiceName", ResourceType = typeof(Resources.EntityValidation))]
        public long? ServiceId { get; set; }
        [Display(Name = "ServiceName", ResourceType = typeof(Resources.EntityValidation))]
        public Service Services { get; set; }
        [Display(Name = "OrderDate", ResourceType = typeof(Resources.EntityValidation))]
        public DateTime? ContactDate { get; set; }
        public long? UserId { get; set; }

        [Display(Name = "OrderStatus", ResourceType = typeof(Resources.EntityValidation))]
        [ForeignKey("OrderStatus")]
        public int? StatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Resources.EntityValidation))]
        public double? Price { get; set; }

        public int? PaymentMethod { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string CookieID { get; set; }

        public bool? IsAgree { get; set; }
    }
}
