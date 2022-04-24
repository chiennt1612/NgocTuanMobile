using Decryptor;
using EntityFramework.API.DBContext;
using EntityFramework.API.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MobileAPI.Helper
{
    public static class StartupHelpers
    {
        #region DbContext
        public static void RegisterDbContexts(this IServiceCollection services, IConfiguration configuration, string migrationsAssembly)
        {
            // Build the intermediate service provider
            var sp = services.BuildServiceProvider();

            // This will succeed.
            var decryptor = sp.GetService<IDecryptorProvider>();
            services.RegisterDbContexts<AppDbContext>(configuration, decryptor, migrationsAssembly);
            //services.RegisterDbContexts<OrderDbContext>(configuration, decryptor);

            services.AddIdSHealthChecks<AppDbContext>(configuration, decryptor);
            //services.AddIdSHealthChecks<OrderDbContext>(configuration, decryptor);
        }
        #endregion
    }
}
