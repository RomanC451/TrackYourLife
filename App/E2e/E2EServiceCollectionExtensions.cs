using Microsoft.Extensions.DependencyInjection.Extensions;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Youtube.Application.Services;

namespace TrackYourLife.App.E2e;

internal static class E2EServiceCollectionExtensions
{
    public static IServiceCollection AddE2EMocks(this IServiceCollection services)
    {
        services.RemoveAll<IStripeService>();
        services.AddSingleton<IStripeService, E2EStripeService>();

        services.RemoveAll<IYoutubeApiService>();
        services.AddScoped<IYoutubeApiService, E2EYoutubeApiService>();

        return services;
    }
}
