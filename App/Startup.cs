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
                .AddJsonFile("appsettings.Development.json", optional: false)
                .AddJsonFile("appsettings.Users.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Nutrition.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_configuration);

        services.AddHttpsRedirection(options =>
        {
            options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            options.HttpsPort = 7196;
        });

        // Application services
        services.AddCommonApplicationServices(_configuration);
        services.AddNutritionApplicationServices();
        services.AddUsersApplicationServices();
        services.AddTrainingsApplicationServices();

        // Infrastructure services
        services.AddCommonInfrastructureServices(_configuration);
        services.AddNutritionInfrastructureServices();
        services.AddUsersInfrastructureServices(_configuration);
        services.AddTrainingsInfrastructureServices();

        // Presentation services
        services.AddCommonPresentationServices(_configuration);
        services.AddUsersPresentationServices();
        services.AddNutritionPresentationServices();
        services.AddTrainingsPresentationServices();
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

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("CORSPolicy");
        app.UseMiddleware<RequestLogContextMiddleware>();
        app.UseMiddleware<AuthorizationBlackListMiddleware>();

        //Infrastructure app config
        app.ConfigureCommonInfrastructureApp(env);
        app.ConfigureUsersInfrastructureApp(env);
        app.ConfigureNutritionInfrastructureApp(env, applicationLifetime, schedulerFactory);
        app.ConfigureTrainingsInfrastructureApp(env);

        //Presentation app config
        app.ConfigureCommonPresentationApp(_configuration);
        app.ConfigureUsersPresentationApp();
        app.ConfigureNutritionPresentationApp();
        app.ConfigureTrainingsPresentationApp();
    }
}
