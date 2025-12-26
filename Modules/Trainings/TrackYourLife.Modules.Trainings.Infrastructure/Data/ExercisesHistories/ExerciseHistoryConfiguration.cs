using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories;

internal sealed class ExerciseHistoryConfiguration : IEntityTypeConfiguration<ExerciseHistory>
{
    public void Configure(EntityTypeBuilder<ExerciseHistory> builder)
    {
        builder.ToTable(TableNames.ExerciseHistory);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OngoingTrainingId).IsRequired();

        builder.Property(x => x.ExerciseId).IsRequired();

        builder.Ignore(x => x.NewExerciseSets);

        builder.Property(x => x.NewExerciseSetsJson).IsRequired();

        builder.Ignore(x => x.OldExerciseSets);

        builder.Property(x => x.OldExerciseSetsJson).IsRequired();

        builder.Property(x => x.AreChangesApplied).IsRequired();

        builder.Property(x => x.CreatedOnUtc).IsRequired();

        builder.Property(x => x.ModifiedOnUtc).IsRequired(false);

        builder
            .HasOne<OngoingTraining>()
            .WithMany()
            .HasForeignKey(x => x.OngoingTrainingId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .HasOne<Exercise>()
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
