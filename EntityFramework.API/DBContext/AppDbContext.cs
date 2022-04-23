using EntityFramework.API.Constants;
using EntityFramework.API.DBContext.Interfaces;
using EntityFramework.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.API.DBContext
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> Status { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<ParamSetting> ParamSettings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureLogContext(builder);
        }

        private void ConfigureLogContext(ModelBuilder builder)
        {
            builder.Entity<Contact>(log =>
            {
                log.ToTable(TableConsts.Contact);
                log.HasKey(x => x.Id);
                log.HasOne(p => p.Services)
                    .WithMany(t => t.Contacts)
                    .HasForeignKey(m => m.ServiceId)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            builder.Entity<About>(log =>
            {
                log.ToTable(TableConsts.About);
                log.HasKey(x => x.Id);
            });
            builder.Entity<Service>(log =>
            {
                log.ToTable(TableConsts.Service);
                log.Property("GroupIdList").IsRequired(false);
                log.HasKey(x => x.Id);
            });
            builder.Entity<FAQ>(log =>
            {
                log.ToTable(TableConsts.FAQ);
                log.HasKey(x => x.Id);
            });

            builder.Entity<ParamSetting>(log =>
            {
                log.ToTable(TableConsts.ParamSetting);
                log.HasKey(x => x.Id);
                log.HasIndex(p => new { p.ParamKey, p.Language }).IsUnique();
            });

            builder.Entity<OrderStatus>(log =>
            {
                log.ToTable(TableConsts.OrderStatus);
                log.HasKey(x => x.Id);
            });

            builder.Entity<Order>(log =>
            {
                log.ToTable(TableConsts.Order);
                log.HasKey(x => x.Id);
            });
        }
    }
}
