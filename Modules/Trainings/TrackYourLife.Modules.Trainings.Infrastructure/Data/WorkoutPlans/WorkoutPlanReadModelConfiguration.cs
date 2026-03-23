using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans;

internal sealed class WorkoutPlanReadModelConfiguration
    : IEntityTypeConfiguration<WorkoutPlanReadModel>
{
    public void Configure(EntityTypeBuilder<WorkoutPlanReadModel> builder)
    {
        builder.ToTable(TableNames.WorkoutPlan);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId);
        builder.Property(x => x.Name);
        builder.Property(x => x.IsActive);
        builder.Property(x => x.CreatedOnUtc);
        builder.Property(x => x.ModifiedOnUtc);

        // Configure unidirectional relationship explicitly to use WorkoutPlanId FK column.
        // Otherwise EF creates a shadow FK like WorkoutPlanReadModelId, which doesn't exist in the migration.
        builder
            .HasMany(x => x.WorkoutPlanTrainings)
            .WithOne()
            .HasForeignKey(x => x.WorkoutPlanId);
    }
}
