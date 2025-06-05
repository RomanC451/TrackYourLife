using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.Extensions;
using TrackYourLife.SharedLib.Infrastructure.Outbox;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data;

/// <summary>
/// Represents the database context for writing nutrition-related data.
/// </summary>
public sealed class TrainingsWriteDbContext(
    DbContextOptions<TrainingsWriteDbContext> options,
    IConfiguration? configuration
) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<Training> Trainings { get; set; }
    public DbSet<TrainingExercise> TrainingExercises { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<OngoingTraining> OngoingTrainings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder
                .UseNpgsql(
                    _configuration.GetConnectionString("Database"),
                    opts =>
                        opts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Trainings")
                )
                .EnableSensitiveDataLogging()
                .AddInterceptors(new ConvertDomainEventsToOutboxMessagesInterceptor());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Trainings");
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            WriteConfigurationsFilter
        );

        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool WriteConfigurationsFilter(Type type) =>
        !type.Name?.Contains("ReadModel") ?? false;
}
