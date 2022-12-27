using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;

namespace MobileAPI
{
    public class Program
    {
        public static int Main(string[] args)
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
                          Directory.GetCurrentDirectory() + @"\Log\API.txt",
                          fileSizeLimitBytes: 10000000,
                          rollOnFileSizeLimit: true,
                          rollingInterval: RollingInterval.Hour,
                          shared: true,
                          flushToDiskInterval: TimeSpan.FromSeconds(1),
                          outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine})")
                      .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                      .CreateLogger();
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
