using EntityFramework.API.Entities;
using EntityFramework.API.Entities.Ordering;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.API.DBContext.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> Status { get; set; }

        public DbSet<InvoiceSave> InvoiceSaves { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<ParamSetting> ParamSettings { get; set; }
    }
}
