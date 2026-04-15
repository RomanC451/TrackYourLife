using System.Net.Http.Headers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Infrastructure.Data;
using TrackYourLife.Modules.Youtube.Infrastructure.Options;
using TrackYourLife.Modules.Youtube.Infrastructure.Services;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Youtube.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddYoutubeInfrastructureServices(
        this IServiceCollection services
    )
    {
        // Add validators
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        // Add options
        services.AddOptionsWithFluentValidation<YoutubeApiOptions>(
            YoutubeApiOptions.ConfigurationSection
        );

        services
            .AddHttpClient<IPipedApiClient, PipedApiClient>()
            .ConfigureHttpClient(
                (serviceProvider, httpClient) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<YoutubeApiOptions>>()
                        .Value;

                    httpClient.BaseAddress = new Uri($"{options.PipedApiBaseUrl.TrimEnd('/')}/");
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json")
                    );
                }
            );

        // Add memory cache for YouTube API responses
        services.AddMemoryCache();

        // Add db contexts
        services.AddDbContext<YoutubeWriteDbContext>();
        services.AddDbContext<YoutubeReadDbContext>();

        // Add repositories
        services.RegisterRepositoriesAndQueries(AssemblyReference.Assembly);

        services.AddScoped<IYoutubeUnitOfWork, YoutubeUnitOfWork>();

        // Add services
        services.AddScoped<IYoutubeApiService, YoutubeApiService>();

        return services;
    }
}
