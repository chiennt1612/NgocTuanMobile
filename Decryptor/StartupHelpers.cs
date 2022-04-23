using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using Utils.Models;

namespace Utils
{
    public static class StartupHelpers
    {
        #region Localization
        private static string[] LanguageSupport = new[] { "vi" };//, "en-US"
        public static void AddServiceLanguage(this IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.SetDefaultCulture(LanguageSupport[0])
                    .AddSupportedCultures(LanguageSupport)
                    .AddSupportedUICultures(LanguageSupport);
            });
        }

        public static void UseConfigLanguage(this IApplicationBuilder app)
        {
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(LanguageSupport[0])
                .AddSupportedCultures(LanguageSupport)
                .AddSupportedUICultures(LanguageSupport);

            app.UseRequestLocalization(localizationOptions);
        }
        #endregion

        public static void AddAuthenticationToken(this IServiceCollection services, IConfiguration configuration)
        {
            ////services.AddAuthentication("token")
            ////    //JWT tokens
            ////    .AddJwtBearer("token", options =>
            ////    {
            ////        options.Authority = Constants.Authority;
            ////        options.Audience = "api2";
            ////        //options.va
            ////        options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
            ////        options.TokenValidationParameters.ValidateIssuer = true;
            ////        options.TokenValidationParameters.ValidateAudience = true;
            ////        options.TokenValidationParameters.RequireExpirationTime = true;
            ////        //if token does not contain a dot, it is a reference tokenauthorize
            ////        options.ForwardDefaultSelector = Selector.ForwardReferenceToken("introspection");
            ////    })
            ////    //reference tokens
            ////    .AddOAuth2Introspection("introspection", options =>
            ////    {
            ////        options.Authority = Constants.Authority;
            ////        options.ClientId = "demo_api_swagger";
            ////        options.ClientSecret = "secretchatapi";
            ////    });
            ////services.AddScopeTransformation();


            var jwtSection = configuration.GetSection("jwt");
            var jwtOptions = new JwtOptionsModel();
            jwtSection.Bind(jwtOptions);
            services.Configure<JwtOptionsModel>(jwtSection);

            // Add bearer authentication
            services.AddAuthentication("token")
                .AddJwtBearer("token", cfg =>
                {
                    cfg.RequireHttpsMetadata = true;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
                        ValidIssuer = jwtOptions.ValidIssuer,
                        ValidAudience = jwtOptions.ValidAudience,
                        // Validate the token expiry  
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }
    }
}
