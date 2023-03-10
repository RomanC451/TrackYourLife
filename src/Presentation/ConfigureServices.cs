namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddPresentationServices(this IServiceCollection services)
    {
        services
            .AddControllers()
            .AddApplicationPart(TrackYourLifeDotnet.Presentation.AssemblyReference.Assembly);
        return services;
    }
}
