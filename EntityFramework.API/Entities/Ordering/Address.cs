using System.ComponentModel.DataAnnotations;

namespace EntityFramework.API.Entities.Ordering
{
    public class Address
    {
        public Address() { }
        public int Id { get; set; }

        public long? UserId { get; set; }

        [Display(Name = "Street", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(50)]
        public string Street { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(50)]
        public string City { get; set; }

        [Display(Name = "State", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(50)]
        public string State { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(50)]
        public string Country { get; set; }

        [Display(Name = "ZipCode", ResourceType = typeof(Resources.EntityValidation))]
        [StringLength(10)]
        public string ZipCode { get; set; }

        //public List<Order> Orders { get; set; }
    }
}
