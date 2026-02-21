using Microsoft.Extensions.DependencyInjection;

namespace TrackYourLife.Modules.Payments.Presentation;

public static class ConfigureServices
{
    public static IServiceCollection AddPaymentsPresentationServices(
        this IServiceCollection services
    )
    {
        return services;
    }
}
