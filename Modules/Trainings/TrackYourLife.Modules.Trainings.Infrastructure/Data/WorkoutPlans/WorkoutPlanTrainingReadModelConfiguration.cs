using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.WorkoutPlans;

internal sealed class WorkoutPlanTrainingReadModelConfiguration
    : IEntityTypeConfiguration<WorkoutPlanTrainingReadModel>
{
    public void Configure(EntityTypeBuilder<WorkoutPlanTrainingReadModel> builder)
    {
        builder.ToTable(TableNames.WorkoutPlanTraining);

        builder.HasKey(x => new { x.WorkoutPlanId, x.TrainingId });

        builder.Property(x => x.OrderIndex);

        builder.HasOne(x => x.Training).WithMany().HasForeignKey(x => x.TrainingId);
    }
}
