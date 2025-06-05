using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data;

/// <summary>
/// Represents the database context for reading trainings-related data.
/// </summary>
public sealed class TrainingsReadDbContext(
    DbContextOptions<TrainingsReadDbContext> options,
    IConfiguration? configuration
) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<TrainingReadModel> Trainings { get; set; }
    public DbSet<TrainingExerciseReadModel> TrainingExercises { get; set; }
    public DbSet<ExerciseReadModel> Exercises { get; set; }
    public DbSet<OngoingTrainingReadModel> OngoingTrainings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder
                .UseNpgsql(_configuration.GetConnectionString("Database"))
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Trainings");

        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            ReadConfigurationsFilter
        );

        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool ReadConfigurationsFilter(Type type) =>
        type.Name?.Contains("ReadModel") ?? false;
}
