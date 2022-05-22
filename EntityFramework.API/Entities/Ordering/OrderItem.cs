using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework.API.Entities.Ordering
{
    public class OrderItem
    {
        public long Id { get; set; }

        [ForeignKey("Product")]
        [Display(Name = "ProductName", ResourceType = typeof(Resources.EntityValidation))]
        public long ProductId { get; set; }
        [Display(Name = "ProductName", ResourceType = typeof(Resources.EntityValidation))]
        public Product Product { get; set; }

        [Display(Name = "Units", ResourceType = typeof(Resources.EntityValidation))]
        public int Units { get; set; } = 1;

        [Display(Name = "Price", ResourceType = typeof(Resources.EntityValidation))]
        public double Price { get; set; }

        [Display(Name = "Discount", ResourceType = typeof(Resources.EntityValidation))]
        public double Discount { get; set; }

        public double? Total { get; set; }

        [ForeignKey("Order")]
        public long OrderId { get; set; }
        public Order Order { get; set; }
    }
}
