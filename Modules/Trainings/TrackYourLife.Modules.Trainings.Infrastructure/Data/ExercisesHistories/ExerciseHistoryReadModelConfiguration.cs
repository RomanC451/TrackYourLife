using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories;

internal sealed class ExerciseHistoryReadModelConfiguration
    : IEntityTypeConfiguration<ExerciseHistoryReadModel>
{
    public void Configure(EntityTypeBuilder<ExerciseHistoryReadModel> builder)
    {
        builder.ToTable(TableNames.ExerciseHistory);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ExerciseId).IsRequired();

        builder.Ignore(x => x.ExerciseSetChanges);

        builder.Property(x => x.ExerciseSetChangesJson).IsRequired();

        builder.Ignore(x => x.ExerciseSetsBeforeChange);

        builder.Property(x => x.ExerciseSetsBeforeChangeJson).IsRequired();

        builder.Property(x => x.AreChangesApplied).IsRequired();

        builder.Property(x => x.CreatedOnUtc).IsRequired();

        builder.Property(x => x.ModifiedOnUtc).IsRequired(false);

        builder
            .HasOne<OngoingTrainingReadModel>()
            .WithMany()
            .HasForeignKey(x => x.OngoingTrainingId)
            .IsRequired();

        builder
            .HasOne<ExerciseReadModel>()
            .WithMany()
            .HasForeignKey(x => x.ExerciseId)
            .IsRequired();
    }
}
