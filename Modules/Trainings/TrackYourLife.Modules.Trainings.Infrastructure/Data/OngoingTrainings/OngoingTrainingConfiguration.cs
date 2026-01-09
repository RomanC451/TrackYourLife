using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings;

internal sealed class OngoingTrainingConfiguration : IEntityTypeConfiguration<OngoingTraining>
{
    public void Configure(EntityTypeBuilder<OngoingTraining> builder)
    {
        builder.ToTable(TableNames.OngoingTraining);

        builder.HasKey(ot => ot.Id);

        builder.Property(ot => ot.UserId).IsRequired();

        builder.Property(ot => ot.ExerciseIndex).IsRequired();

        builder.Property(ot => ot.SetIndex).IsRequired();

        builder.Property(ot => ot.StartedOnUtc).IsRequired();

        builder.Property(ot => ot.FinishedOnUtc).IsRequired(false);

        builder.HasOne(ot => ot.Training).WithMany().IsRequired().OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(ot => ot.CurrentExercise);
        builder.Ignore(ot => ot.ExercisesCount);
        builder.Ignore(ot => ot.SetsCount);
        builder.Ignore(ot => ot.IsFinished);
        builder.Ignore(ot => ot.IsLastSet);
        builder.Ignore(ot => ot.IsLastExercise);
        builder.Ignore(ot => ot.IsLastSetAndExercise);
    }
}
