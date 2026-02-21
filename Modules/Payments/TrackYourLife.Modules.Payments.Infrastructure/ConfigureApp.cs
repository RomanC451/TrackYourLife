using Microsoft.AspNetCore.Builder;

using Microsoft.AspNetCore.Hosting;

namespace TrackYourLife.Modules.Payments.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigurePaymentsInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        // Infrastructure app config
    }
}
