using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TrackYourLife.Modules.Reading.Infrastructure.Data;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Reading.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigureReadingInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        if (env.IsDevelopment() || env.IsEnvironment("Testing"))
        {
            app.ApplyMigrations<ReadingWriteDbContext>();
        }
    }
}
