using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using StaffAPI.Helper;
using System;
using System.IO;

namespace StaffAPI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            LoggingProvider settings_Web = new LoggingProvider();
            config.GetSection("LoggingProvider").Bind(settings_Web);
            if (settings_Web.LoggingType == 1)
            {
                Log.Logger = new LoggerConfiguration()
                      .MinimumLevel.Debug()
                      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                      .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                      .MinimumLevel.Override("System", LogEventLevel.Warning)
                      .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                      .Enrich.FromLogContext()
                      // uncomment to write to Azure diagnostics stream
                      .WriteTo.MongoDB(settings_Web.LogMongoDB.URI, settings_Web.LogMongoDB.Collection)
                      .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                      .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                      .MinimumLevel.Debug()
                      .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                      .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                      .MinimumLevel.Override("System", LogEventLevel.Warning)
                      .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                      .Enrich.FromLogContext()
                      // uncomment to write to Azure diagnostics stream
                      .WriteTo.File(
                          Directory.GetCurrentDirectory() + settings_Web.LogFiles.FileName,
                          fileSizeLimitBytes: settings_Web.LogFiles.FileSizeLimitBytes,
                          rollOnFileSizeLimit: true,
                          rollingInterval: (Serilog.RollingInterval)settings_Web.LogFiles.RollingInterval,
                          shared: true,
                          flushToDiskInterval: TimeSpan.FromSeconds(1),
                          outputTemplate: settings_Web.LogFiles.OutputTemplate)
                      .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                      .CreateLogger();
            }
            var host = CreateHostBuilder(args).Build();
            Log.Information("Starting host...");
            host.Run();
            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
