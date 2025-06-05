using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.TrainingExercises;

internal sealed class TrainingExerciseConfiguration : IEntityTypeConfiguration<TrainingExercise>
{
    public void Configure(EntityTypeBuilder<TrainingExercise> builder)
    {
        builder.ToTable(TableNames.TrainingExercise);

        builder.HasKey(te => new { te.TrainingId, te.ExerciseId });

        builder.Property(te => te.TrainingId).IsRequired();

        builder.Property(te => te.OrderIndex).IsRequired();

        builder
            .HasOne(te => te.Exercise)
            .WithMany()
            .HasForeignKey(te => te.ExerciseId)
            .IsRequired();
    }
}
