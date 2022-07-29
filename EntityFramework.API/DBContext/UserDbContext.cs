using EntityFramework.API.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.API.DBContext
{
    public class UserDbContext : IdentityDbContext<AppUser, AppRole, long>
    {
        public DbSet<AppUserDevice> AppUserDevices { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options)
               : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<AppUser>(u1 =>
            {
                u1.HasKey(u => u.Id);
                u1.Property(u => u.Email)
                    .HasMaxLength(200)
                    .IsRequired(false);
                //u1.HasIndex(u => u.Email).IsUnique();

                u1.Property(u => u.UserName)
                    .HasMaxLength(30)
                    .IsRequired();
                u1.HasIndex(u => u.UserName).IsUnique();

                u1.Property(u => u.PhoneNumber)
                    .HasMaxLength(16);

                u1.ToTable("Users");
            });

            builder.Entity<AppRole>(u1 =>
            {
                u1.HasKey(u => u.Id);

                u1.HasIndex(u => u.Name).IsUnique();
                u1.Property(u => u.Name)
                    .HasMaxLength(128)
                    .IsRequired();

                u1.ToTable("Roles");
            });

            //builder.Entity<UserClaim>(u1 =>
            //{
            //    //u1.HasKey(u => u.Id);
            //    u1.ToTable("UserClaim");
            //});

            //builder.Entity<UserRole>(u1 =>
            //{
            //    //u1.HasKey(u => u.UserId);
            //    //u1.HasKey(u => u.RoleId);
            //    u1.ToTable("UserRole");
            //});

            //builder.Entity<UserLogin>(u1 =>
            //{
            //    //u1.HasKey(u => u.UserId);
            //    u1.ToTable("UserLogin");
            //});

            //builder.Entity<RoleClaim>(u1 =>
            //{
            //    //u1.HasKey(u => u.Id);
            //    u1.ToTable("RoleClaim");
            //});

            //builder.Entity<UserToken>(u1 =>
            //{
            //    //u1.HasKey(u => u.UserId);
            //    u1.ToTable("UserToken");
            //});

            builder.Entity<AppUserDevice>(u1 =>
            {
                u1.HasKey(u => u.Id);

                u1.HasIndex(u => u.DeviceID).IsUnique();
                u1.Property(u => u.DeviceID).HasMaxLength(36);
                u1.Property(u => u.Username).HasMaxLength(30);
                u1.Property(u => u.Token).HasMaxLength(2000);

                u1.ToTable("UserDevices");
            });
        }
    }
}
