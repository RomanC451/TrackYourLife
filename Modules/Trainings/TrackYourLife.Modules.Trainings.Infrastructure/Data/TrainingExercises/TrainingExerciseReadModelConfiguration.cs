using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.TrainingExercises;

internal sealed class TrainingExerciseReadModelConfiguration
    : IEntityTypeConfiguration<TrainingExerciseReadModel>
{
    public void Configure(EntityTypeBuilder<TrainingExerciseReadModel> builder)
    {
        builder.ToTable(TableNames.TrainingExercise);

        builder.HasKey(te => new { te.TrainingId, te.ExerciseId });

        builder.Property(te => te.TrainingId);

        builder.Property(te => te.ExerciseId);

        builder
            .HasOne<TrainingReadModel>()
            .WithMany(t => t.TrainingExercises)
            .HasForeignKey(te => te.TrainingId);

        builder.HasOne(te => te.Exercise).WithMany().HasForeignKey(te => te.ExerciseId);
    }
}
