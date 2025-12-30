using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TrackYourLife.Modules.Youtube.Infrastructure.Data;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Youtube.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigureYoutubeInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        if (env.IsDevelopment() || env.IsEnvironment("Testing"))
        {
            // Apply migrations
            app.ApplyMigrations<YoutubeWriteDbContext>();
        }
    }
}

