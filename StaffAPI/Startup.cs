using StaffAPI.Helper;
using Decryptor;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMSGetway;
using System.Reflection;
using Utils;
using Utils.Tokens;
using Utils.Tokens.Interfaces;

namespace StaffAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddControllers();
            services.AddHttpContextAccessor();
            services.AddServiceLanguage();

            services.AddSenders(Configuration);
            services.AddSingleton<IDecryptorProvider, DecryptorProvider>();
            services.AddSingleton<ISMSVietel, SMSVietel>();
            services.AddSingleton<ITokenCreationService, TokenCreationService>();
            services.AddServices();

            services.RegisterDbContexts(Configuration, migrationsAssembly);
            services.RegisterAuthentication(Configuration);
            //services.AddAuthenticationToken(Configuration);
            services.AddDistributedMemoryCache();
            //services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth v1"));
            //}

            app.UseSwagger("Nuoc Ngoc Tuan Mobile Staff v1.0");

            app.UseHttpsRedirection();

            app.UseConfigLanguage();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();
            //app.UseAntiforgeryToken();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}
