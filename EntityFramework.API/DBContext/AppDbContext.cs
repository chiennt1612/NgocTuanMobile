using EntityFramework.API.Constants;
using EntityFramework.API.DBContext.Interfaces;
using EntityFramework.API.Entities;
using EntityFramework.API.Entities.Ordering;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.API.DBContext
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatus> Status { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<InvoiceSave> InvoiceSaves { get; set; }
        public DbSet<About> Abouts { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<FAQ> FAQs { get; set; }
        public DbSet<Categories> Categoriess { get; set; }
        public DbSet<NewsCategories> NewsCategoriess { get; set; }
        public DbSet<ParamSetting> ParamSettings { get; set; }

        public DbSet<Adv> Advs { get; set; }
        public DbSet<AdvPosition> AdvPositions { get; set; }
        public DbSet<Notice> Notices { get; set; }
        public DbSet<Contract> Contracts { get; set; }

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
            builder.Entity<Adv>(log =>
            {
                log.ToTable(TableConsts.Adv);
                log.HasKey(x => x.Id);
            });
            builder.Entity<AdvPosition>(log =>
            {
                log.ToTable(TableConsts.AdvPosition);
                log.HasKey(x => x.Id);
            });

            builder.Entity<InvoiceSave>(log =>
            {
                log.ToTable(TableConsts.InvoiceSave);
                log.HasKey(x => x.Id);
            });

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


            builder.Entity<Product>(log =>
            {
                log.ToTable(TableConsts.Product);
                log.HasKey(x => x.Id);

                log.HasOne(p => p.MainCategories)
                    .WithMany(t => t.Products)
                    .HasForeignKey(m => m.CategoryMain)
                    .OnDelete(DeleteBehavior.NoAction);

                log.HasOne(p => p.ReferCategories)
                    .WithMany(t => t.ReferProducts)
                    .HasForeignKey(m => m.CategoryReference)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Article>(log =>
            {
                log.ToTable(TableConsts.Article);
                log.HasKey(x => x.Id);

                log.HasOne(p => p.MainCategories)
                    .WithMany(t => t.Articles)
                    .HasForeignKey(m => m.CategoryMain)
                    .OnDelete(DeleteBehavior.Cascade);

                log.HasOne(p => p.ReferCategories)
                    .WithMany(t => t.ReferArticles)
                    .HasForeignKey(m => m.CategoryReference)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<Categories>(log =>
            {
                log.ToTable(TableConsts.Categories);
                log.Property(x => x.ParentId).IsRequired(false);
                log.HasKey(x => x.Id);

            });

            builder.Entity<NewsCategories>(log =>
            {
                log.ToTable(TableConsts.NewsCategories);
                log.Property(x => x.ParentId).IsRequired(false);
                log.HasKey(x => x.Id);
            });

            builder.Entity<Address>(log =>
            {
                log.ToTable(TableConsts.Address);
                log.HasKey(x => x.Id);
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

            builder.Entity<OrderItem>(log =>
            {
                log.ToTable(TableConsts.OrderItem);
                log.HasKey(x => x.Id);
            });

            builder.Entity<Notice>(u1 =>
            {
                u1.HasKey(u => u.Id);

                u1.HasIndex(u => u.DeviceID);
                u1.HasIndex(u => u.Username);
                //u1.HasIndex(u => u.CreateDate);
                u1.Property(u => u.DeviceID).HasMaxLength(36);
                u1.Property(u => u.Username).HasMaxLength(30);

                u1.Property(u => u.NoticeTypeName).HasMaxLength(128);
                u1.Property(u => u.Author).HasMaxLength(128);
                u1.Property(u => u.Subject).HasMaxLength(200);
                u1.Property(u => u.Content).HasMaxLength(500);

                u1.ToTable("UserNotice");
            });

            builder.Entity<Contract>(u1 =>
            {
                u1.HasKey(u => u.Id);

                u1.ToTable("UserContract");
            });
        }
    }
}
