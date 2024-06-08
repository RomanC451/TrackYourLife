using TrackYourLife.Common.Persistence;
using TrackYourLife.Common.Application;
using TrackYourLife.Common.Infrastructure;
using TrackYourLife.Common.Presentation;

namespace TrackYourLifeDotnet.App;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationServices(Configuration);
        services.AddInfrastructureServices(Configuration);
        services.AddPersistenceServices();
        services.AddPresentationServices(Configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.ConfigureInfrastructureApp(env);
        app.ConfigurePersistenceApp();
        app.ConfigurePresentationApp();
    }
}
