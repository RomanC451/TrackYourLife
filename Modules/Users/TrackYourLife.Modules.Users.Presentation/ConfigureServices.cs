using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.Modules.Users.Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddUsersPresentationServices(this IServiceCollection services)
    {
        // services
        //     .AddControllers()
        //     .AddJsonOptions(options =>
        //     {
        //         options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        //     })
        //     .AddApplicationPart(AssemblyReference.Assembly);

        return services;
    }
}
