using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Quartz;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Domain.Core;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.BackgroundJobs;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Data;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Options;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Services;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddGuitarTabsInfrastructureServices(
        this IServiceCollection services
    )
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        services.AddDbContext<GuitarTabsWriteDbContext>();
        services.AddDbContext<GuitarTabsReadDbContext>();

        services.RegisterRepositoriesAndQueries(AssemblyReference.Assembly);

        services.AddScoped<IGuitarTabsUnitOfWork, GuitarTabsUnitOfWork>();

        services.AddOptionsWithFluentValidation<UltimateGuitarOptions>(
            UltimateGuitarOptions.ConfigurationSection
        );

        services
            .AddHttpClient<IUltimateGuitarSearchClient, UltimateGuitarSearchClient>()
            .ConfigureHttpClient(
                (serviceProvider, client) =>
                {
                    var options = serviceProvider
                        .GetRequiredService<IOptions<UltimateGuitarOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(options.SearchBaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(20);
                    client.DefaultRequestHeaders.TryAddWithoutValidation(
                        "User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36"
                    );
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html");
                }
            );

        services.AddQuartz(configure =>
        {
            var outboxJobKey = new JobKey(nameof(ProcessGuitarTabsOutboxMessagesJob));

            configure
                .AddJob<ProcessGuitarTabsOutboxMessagesJob>(outboxJobKey)
                .AddTrigger(trigger =>
                    trigger
                        .ForJob(outboxJobKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(10).RepeatForever()
                        )
                );
        });

        return services;
    }
}
