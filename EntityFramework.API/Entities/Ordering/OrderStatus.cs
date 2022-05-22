using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.API.Entities.Ordering
{
    public class OrderStatus
    {
        [Display(Name = "Name", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(50)]
        public string Name { get; set; }
        public int Id { get; set; }

        public List<Order> Orders { get; set; }
        public List<Contact> Contacts { get; set; }
        //protected OrderStatus(int id, string name) => (Id, Name) = (id, name);
        //public static OrderStatus Created = new OrderStatus(0, nameof(Created).ToLowerInvariant());
        //public static OrderStatus Submitted = new OrderStatus(1, nameof(Submitted).ToLowerInvariant());
        //public static OrderStatus AwaitingValidation = new OrderStatus(2, nameof(AwaitingValidation).ToLowerInvariant());
        //public static OrderStatus StockConfirmed = new OrderStatus(3, nameof(StockConfirmed).ToLowerInvariant());
        //public static OrderStatus Paid = new OrderStatus(4, nameof(Paid).ToLowerInvariant());
        //public static OrderStatus Shipped = new OrderStatus(5, nameof(Shipped).ToLowerInvariant());
        //public static OrderStatus Cancelled = new OrderStatus(6, nameof(Cancelled).ToLowerInvariant());

        //public static IEnumerable<OrderStatus> GetAll() =>
        //            typeof(OrderStatus).GetFields(BindingFlags.Public |
        //                                BindingFlags.Static |
        //                                BindingFlags.DeclaredOnly)
        //                     .Select(f => f.GetValue(null))
        //                     .Cast<OrderStatus>();
    }
}
