using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans;

internal sealed class WorkoutPlanTrainingConfiguration
    : IEntityTypeConfiguration<WorkoutPlanTraining>
{
    public void Configure(EntityTypeBuilder<WorkoutPlanTraining> builder)
    {
        builder.ToTable(TableNames.WorkoutPlanTraining);

        builder.HasKey(x => new { x.WorkoutPlanId, x.TrainingId });

        builder.Property(x => x.OrderIndex).IsRequired();

        builder.HasOne<Training>().WithMany().HasForeignKey(x => x.TrainingId);
    }
}
