using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Infrastructure.BackgroundJobs;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Trainings.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddTrainingsInfrastructureServices(
        this IServiceCollection services
    )
    {
        // Add validators
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        // Add options
        // services.AddOptionsWithFluentValidation<FoodApiOptions>(
        //     FoodApiOptions.ConfigurationSection
        // );

        //Add db contexts
        services.AddDbContext<TrainingsWriteDbContext>();
        services.AddDbContext<TrainingsReadDbContext>();

        //Add repositories
        services.RegisterRepositories(AssemblyReference.Assembly);

        services.AddScoped<ITrainingsUnitOfWork, TrainingsUnitOfWork>();

        //Add Background jobs
        services.AddQuartz(configure =>
        {
            var outboxJobKey = new JobKey($"{nameof(ProcessTrainingsOutboxMessagesJob)}");

            configure
                .AddJob<ProcessTrainingsOutboxMessagesJob>(outboxJobKey)
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
