using Decryptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EntityFramework.API.Helper
{
    public static class DatabaseExtensions
    {
        public static void RegisterDbContexts<TDbContext>
                   (this IServiceCollection services, IConfiguration configuration, IDecryptorProvider decryptor, string migrationsAssembly)
               where TDbContext : DbContext
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            //string connectionString = decryptor.Decrypt(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            //var migrationsAssembly = typeof(DatabaseExtensions).GetTypeInfo().Assembly.GetName().Name;

            // Config DB for identity
            services.AddDbContext<TDbContext>(options => options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));
        }

        public static void AddIdSHealthChecks<TDbContext>
            (this IServiceCollection services, IConfiguration configuration, IDecryptorProvider decryptor)
            where TDbContext : DbContext
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            //string connectionString = decryptor.Decrypt(Environment.GetEnvironmentVariable("CONNECTION_STRING"));

            services.AddHealthChecks().AddSqlServer(connectionString);
            var healthChecksBuilder = services.AddHealthChecks().AddDbContextCheck<TDbContext>();

            var serviceProvider = services.BuildServiceProvider();
            var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var dataAppTableName = DbContextHelpers.GetEntityTable<TDbContext>(scope.ServiceProvider);

                healthChecksBuilder
                            .AddSqlServer(connectionString, name: "AppDbContext1",
                                healthQuery: $"SELECT TOP 1 * FROM dbo.[{dataAppTableName}]");
            }
        }
    }
}
