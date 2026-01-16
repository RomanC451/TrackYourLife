using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings;

internal sealed class OngoingTrainingReadModelConfiguration
    : IEntityTypeConfiguration<OngoingTrainingReadModel>
{
    public void Configure(EntityTypeBuilder<OngoingTrainingReadModel> builder)
    {
        builder.ToTable(TableNames.OngoingTraining);

        builder.HasKey(ot => ot.Id);

        builder.Property(ot => ot.UserId);

        builder.Property(ot => ot.ExercisesCount);

        builder.Property(ot => ot.ExerciseIndex);

        builder.Property(ot => ot.SetsCount);

        builder.Property(ot => ot.SetIndex);

        builder.Property(ot => ot.StartedOnUtc);

        builder.Property(ot => ot.FinishedOnUtc);

        builder.Property(ot => ot.CaloriesBurned);

        builder.HasOne(ot => ot.Training).WithMany();

        builder.Ignore(ot => ot.CurrentExercise);
        builder.Ignore(ot => ot.ExercisesCount);
        builder.Ignore(ot => ot.SetsCount);
        builder.Ignore(ot => ot.IsFinished);
        builder.Ignore(ot => ot.IsLastSet);
        builder.Ignore(ot => ot.IsLastExercise);
        builder.Ignore(ot => ot.IsLastSetAndExercise);
    }
}
