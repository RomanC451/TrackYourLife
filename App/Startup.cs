using Quartz;
using TrackYourLife.Modules.Common.Application;
using TrackYourLife.Modules.Common.Infrastructure;
using TrackYourLife.Modules.Common.Presentation;
using TrackYourLife.Modules.Nutrition.Application;
using TrackYourLife.Modules.Nutrition.Infrastructure;
using TrackYourLife.Modules.Nutrition.Presentation;
using TrackYourLife.Modules.Users.Application;
using TrackYourLife.Modules.Users.Infrastructure;
using TrackYourLife.Modules.Users.Presentation;

namespace TrackYourLifeDotnet.App;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup()
    {
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: false)
            .AddJsonFile("appsettings.Users.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Nutrition.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_configuration);

        // Application services
        services.AddCommonApplicationServices(_configuration);
        services.AddNutritionApplicationServices();
        services.AddUsersApplicationServices();

        // Infrastructure services
        services.AddCommonInfrastructureServices(_configuration);
        services.AddNutritionInfrastructureServices();
        services.AddUsersInfrastructureServices(_configuration);

        // Presentation services
        services.AddCommonPresentationServices(_configuration);
        services.AddNutritionPresentationServices();
        services.AddUsersPresentationServices();
    }

    public static void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IHostApplicationLifetime applicationLifetime,
        ISchedulerFactory schedulerFactory
    )
    {
        //Infrastructure app config
        app.ConfigureCommonInfrastructureApp(env);
        app.ConfigureNutritionInfrastructureApp(env, applicationLifetime, schedulerFactory);
        app.ConfigureUsersInfrastructureApp(env);

        //Presentation app config
        app.ConfigureCommonPresentationApp();
        app.ConfigureNutritionPresentationApp();
    }
}
