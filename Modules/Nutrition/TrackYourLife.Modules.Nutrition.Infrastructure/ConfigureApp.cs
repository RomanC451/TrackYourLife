using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Quartz;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.ShutdownJobs;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Nutrition.Infrastructure;

public static class ConfigureApp
{
    public static void ConfigureNutritionInfrastructureApp(
        this IApplicationBuilder app,
        IWebHostEnvironment env,
        IHostApplicationLifetime applicationLifetime,
        ISchedulerFactory schedulerFactory
    )
    {
        applicationLifetime.ApplicationStopping.Register(async () =>
        {
            var scheduler = await schedulerFactory.GetScheduler();
            var jobKey = new JobKey(nameof(SaveFoodApiCookiesJob));
            await scheduler.TriggerJob(jobKey);
        });

        if (env.IsDevelopment())
        {
            //Apply migrations
            app.ApplyMigrations<NutritionWriteDbContext>();
        }
    }
}
