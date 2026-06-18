using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using TrackYourLife.Modules.Reading.Domain.Core;
using TrackYourLife.Modules.Reading.Infrastructure.BackgroundJobs;
using TrackYourLife.Modules.Reading.Infrastructure.Data;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Reading.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddReadingInfrastructureServices(
        this IServiceCollection services
    )
    {
        services.AddValidatorsFromAssembly(AssemblyReference.Assembly, includeInternalTypes: true);

        services.AddDbContext<ReadingWriteDbContext>();
        services.AddDbContext<ReadingReadDbContext>();

        services.RegisterRepositoriesAndQueries(AssemblyReference.Assembly);

        services.AddScoped<IReadingUnitOfWork, ReadingUnitOfWork>();

        services.AddQuartz(configure =>
        {
            var outboxJobKey = new JobKey(nameof(ProcessReadingOutboxMessagesJob));

            configure
                .AddJob<ProcessReadingOutboxMessagesJob>(outboxJobKey)
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
