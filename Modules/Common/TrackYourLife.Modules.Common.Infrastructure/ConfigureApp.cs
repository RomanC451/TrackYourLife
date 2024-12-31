using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TrackYourLife.Modules.Common.Infrastructure.Data;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Common.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigureCommonInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        if (env.IsDevelopment())
        {

            //Apply migrations
            app.ApplyMigrations<CommonWriteDbContext>();
        }
    }
}
