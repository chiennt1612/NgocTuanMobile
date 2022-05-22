using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.API.Entities.Ordering
{
    public class Order
    {
        //[StringLength(50)]
        public string CookieID { get; set; }
        public long Id { get; set; }

        [Display(Name = "OrderDate", ResourceType = typeof(Resources.EntityValidation))]
        public DateTime OrderDate { get; set; }
        public long? UserId { get; set; }
        [StringLength(30)]
        public string UserName { get; set; }

        [Display(Name = "Description", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(2000)]
        public string Description { get; set; }

        //[Display(Name = "Address", ResourceType = typeof(Resources.EntityValidation))]
        //[ForeignKey("Address")]
        //public int? AddressId { get; set; }
        //public Address Address { get; set; }

        [Display(Name = "OrderStatus", ResourceType = typeof(Resources.EntityValidation))]
        [ForeignKey("OrderStatus")]
        public int StatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        /// <Order contact info >
        [StringLength(128, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Fullname", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Fullname { get; set; }

        [StringLength(300, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [Display(Name = "Address", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Address { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        public string Mobile { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLengthTooLong", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        //[Display(Name = "Email", ResourceType = typeof(Resources.EntityValidation))]
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.EntityValidation))]
        [EmailAddress(ErrorMessageResourceName = "EmailIsNotValid", ErrorMessageResourceType = typeof(LanguageAll.Language))]
        public string Email { get; set; }

        public int? PaymentMethod { get; set; }
        public double? Total { get; set; }
        public double? FeeShip { get; set; }
        /// </Order contact info>
        public bool? IsAgree { get; set; }

        public Order()
        {
            OrderItems = new List<OrderItem>();
            StatusId = 0;
            PaymentMethod = 1;
            Total = 0;
            //CookieID = Guid.NewGuid().ToString();
        }
    }
}
