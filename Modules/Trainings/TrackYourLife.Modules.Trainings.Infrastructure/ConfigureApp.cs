using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Trainings.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigureTrainingsInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env
    )
    {
        if (env.IsDevelopment() || env.IsEnvironment("Testing"))
        {
            //Apply migrations
            app.ApplyMigrations<TrainingsWriteDbContext>();
        }
    }
}
