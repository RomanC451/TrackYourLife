using Microsoft.EntityFrameworkCore;
using Serilog;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Users.Infrastructure.Data;

namespace TrackYourLife.App;

public class Program
{
    protected Program() { }

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog(
                (hostingContext, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
                }
            )
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder
                    .UseKestrel(options =>
                    {
                        options.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.SslProtocols =
                                System.Security.Authentication.SslProtocols.Tls12
                                | System.Security.Authentication.SslProtocols.Tls13;
                        });
                    })
                    .UseStartup<Startup>();
            });
}
