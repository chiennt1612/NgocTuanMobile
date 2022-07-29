using Auth.Repository;
using Auth.Repository.Interfaces;
using Auth.Services;
using Auth.Services.Interfaces;
using Decryptor;
using EntityFramework.API.DBContext;
using EntityFramework.API.Entities.Identity;
using EntityFramework.API.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Paygate.OnePay;
using System;
using System.Text;
using Utils.Repository;
using Utils.Repository.Interfaces;

namespace Auth.Helper
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
            services.RegisterDbContexts<UserDbContext, AppDbContext>(configuration, decryptor, migrationsAssembly);
            //services.RegisterDbContexts<OrderDbContext>(configuration, decryptor);

            services.AddIdSHealthChecks<UserDbContext>(configuration, decryptor);
            //services.AddIdSHealthChecks<OrderDbContext>(configuration, decryptor);
        }
        #endregion

        public static void RegisterPaygateService(this IServiceCollection services, IConfiguration configuration)
        {
            // Paygate config
            var paygateInfo = configuration.GetSection(nameof(PaygateInfo)).Get<PaygateInfo>();

            if (paygateInfo == null)
            {
                paygateInfo = new PaygateInfo();
            }

            services.AddSingleton(paygateInfo);
            services.AddSingleton<IVPCRequest, VPCRequest>();
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IUserDeviceRepository, UserDeviceRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();          
            services.AddScoped<IUnitOfWork, UnitOfWork>();            

            services.AddScoped<IContractServices, ContractServices>();
            services.AddScoped<INoticeServices, NoticeServices>();
            services.AddScoped<IInvoiceServices, InvoiceServices>();            
            services.AddScoped<IArticleServices, ArticleServices>();
            services.AddScoped<ICategoriesServices, CategoriesServices>();
            services.AddScoped<INewsCategoriesServices, NewsCategoriesServices>();
            services.AddScoped<IParamSettingServices, ParamSettingServices>();
            services.AddScoped<IProductServices, ProductServices>();
            services.AddScoped<IAboutServices, AboutServices>();
            services.AddScoped<IContactServices, ContactServices>();
            services.AddScoped<IAdvServices, AdvServices>();
            services.AddScoped<IServiceServices, ServiceServices>();
            services.AddScoped<IFAQServices, FAQServices>();
            services.AddScoped<IInvoiceSaveServices, InvoiceSaveServices>();
            services.AddScoped<IProfile, Profile>();

            services.AddScoped<IAllService, AllService>();
        }

        public static void RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthenticationServices<UserDbContext, AppUser, AppRole>(configuration);
        }

        public static void AddAuthenticationServices<TIdentityDbContext, TUserIdentity, TUserIdentityRole>(this IServiceCollection services, IConfiguration configuration)
                where TIdentityDbContext : DbContext
               where TUserIdentity : class
               where TUserIdentityRole : class
        {
            var loginConfiguration = GetLoginConfiguration(configuration);
            var registrationConfiguration = GetRegistrationConfiguration(configuration);
            var identityOptions = configuration.GetSection(nameof(IdentityOptions)).Get<IdentityOptions>();

            services
                .AddSingleton(registrationConfiguration)
                .AddSingleton(loginConfiguration)
                .AddSingleton(identityOptions)
                .AddScoped<UserResolver<TUserIdentity>>()
                .AddIdentity<TUserIdentity, TUserIdentityRole>(options => configuration.GetSection(nameof(IdentityOptions)).Bind(options))
                .AddEntityFrameworkStores<TIdentityDbContext>()
                .AddUserManager<AppUserManager>()
                .AddDefaultTokenProviders();

            // Adding Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
                };
            });

            //services.Configure<CookiePolicyOptions>(options =>
            //{
            //    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            //    options.Secure = CookieSecurePolicy.SameAsRequest;
            //    options.OnAppendCookie = cookieContext =>
            //        AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            //    options.OnDeleteCookie = cookieContext =>
            //        AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            //});

            //services.Configure<IISOptions>(iis =>
            //{
            //    iis.AuthenticationDisplayName = "Windows";
            //    iis.AutomaticAuthentication = false;
            //});
        }

        public static void AddSenders(this IServiceCollection services, IConfiguration configuration)
        {
            var smtpConfiguration = configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();

            if (smtpConfiguration != null && !string.IsNullOrWhiteSpace(smtpConfiguration.Host))
            {
                services.AddSingleton(smtpConfiguration);
                services.AddSingleton<IEmailSender, SmtpEmailSender>();
            }
            else
            {
                services.AddSingleton<IEmailSender, EmailSender>();
            }
        }

        private static LoginConfiguration GetLoginConfiguration(IConfiguration configuration)
        {
            var loginConfiguration = configuration.GetSection(nameof(LoginConfiguration)).Get<LoginConfiguration>();

            // Cannot load configuration - use default configuration values
            if (loginConfiguration == null)
            {
                return new LoginConfiguration();
            }

            return loginConfiguration;
        }

        private static RegisterConfiguration GetRegistrationConfiguration(IConfiguration configuration)
        {
            var registerConfiguration = configuration.GetSection(nameof(RegisterConfiguration)).Get<RegisterConfiguration>();

            // Cannot load configuration - use default configuration values
            if (registerConfiguration == null)
            {
                return new RegisterConfiguration();
            }

            return registerConfiguration;
        }
    }
}
