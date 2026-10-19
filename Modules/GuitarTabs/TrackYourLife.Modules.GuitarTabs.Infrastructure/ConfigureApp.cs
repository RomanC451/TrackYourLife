using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Data;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigureGuitarTabsInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        if (env.IsDevelopment() || env.IsEnvironment("Testing"))
        {
            app.ApplyMigrations<GuitarTabsWriteDbContext>();
        }
    }
}
