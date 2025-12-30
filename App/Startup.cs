using Quartz;
using TrackYourLife.Modules.Common.Application;
using TrackYourLife.Modules.Common.Infrastructure;
using TrackYourLife.Modules.Common.Presentation;
using TrackYourLife.Modules.Common.Presentation.Middlewares;
using TrackYourLife.Modules.Nutrition.Application;
using TrackYourLife.Modules.Nutrition.Infrastructure;
using TrackYourLife.Modules.Nutrition.Presentation;
using TrackYourLife.Modules.Trainings.Application;
using TrackYourLife.Modules.Trainings.Infrastructure;
using TrackYourLife.Modules.Trainings.Presentation;
using TrackYourLife.Modules.Users.Application;
using TrackYourLife.Modules.Users.Infrastructure;
using TrackYourLife.Modules.Users.Presentation;
using TrackYourLife.Modules.Users.Presentation.Middlewares;
using TrackYourLife.Modules.Youtube.Application;
using TrackYourLife.Modules.Youtube.Infrastructure;
using TrackYourLife.Modules.Youtube.Presentation;

namespace TrackYourLife.App;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;

        if (!environment.IsEnvironment("Testing"))
        {
            DotNetEnv.Env.Load();

            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.{environment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true
                )
                .AddJsonFile("appsettings.Users.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.Users.{environment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true
                )
                .AddJsonFile("appsettings.Nutrition.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.Nutrition.{environment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true
                )
                .AddJsonFile("appsettings.Youtube.json", optional: false, reloadOnChange: true)
                .AddJsonFile(
                    $"appsettings.Youtube.{environment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: true
                )
                .AddEnvironmentVariables()
                .Build();
        }
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_configuration);

        // HTTPS redirection is handled by Caddy reverse proxy
        // No need for ASP.NET Core HTTPS redirection
        // Application services
        services.AddCommonApplicationServices(_configuration);
        services.AddNutritionApplicationServices();
        services.AddUsersApplicationServices();
        services.AddTrainingsApplicationServices();
        services.AddYoutubeApplicationServices();

        // Infrastructure services
        services.AddCommonInfrastructureServices(_configuration);
        services.AddNutritionInfrastructureServices();
        services.AddUsersInfrastructureServices(_configuration);
        services.AddTrainingsInfrastructureServices();
        services.AddYoutubeInfrastructureServices();

        // Presentation services
        services.AddCommonPresentationServices(_configuration);
        services.AddUsersPresentationServices();
        services.AddNutritionPresentationServices();
        services.AddTrainingsPresentationServices();
        services.AddYoutubePresentationServices();
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IHostApplicationLifetime applicationLifetime,
        ISchedulerFactory schedulerFactory
    )
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        // HTTPS redirection is handled by Caddy reverse proxy
        app.UseRouting();
        app.UseCors("CORSPolicy");
        app.UseMiddleware<RequestLogContextMiddleware>();
        app.UseMiddleware<AuthorizationBlackListMiddleware>();

        //Infrastructure app config
        app.ConfigureCommonInfrastructureApp(env);
        app.ConfigureUsersInfrastructureApp(env);
        app.ConfigureNutritionInfrastructureApp(env, applicationLifetime, schedulerFactory);
        app.ConfigureTrainingsInfrastructureApp(env);
        app.ConfigureYoutubeInfrastructureApp(env);

        //Presentation app config
        app.ConfigureCommonPresentationApp(_configuration);
        app.ConfigureUsersPresentationApp();
        app.ConfigureNutritionPresentationApp();
        app.ConfigureTrainingsPresentationApp();
        app.ConfigureYoutubePresentationApp();
    }
}

