using Microsoft.Extensions.Hosting;
using Serilog;

namespace TrackYourLife.Common.Infrastructure;

public static class ConfigureHost
{

    public static IHostBuilder ConfigureSerilog(this IHostBuilder host)
    {
        host.UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration);
        });

        return host;
    }
}
