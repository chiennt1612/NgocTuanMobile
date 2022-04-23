using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EntityFramework.API.Entities
{
    public class OrderStatus
    {
        [Display(Name = "Name", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(50)]
        public string Name { get; set; }
        public int Id { get; set; }

        public List<Order> Orders { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}
