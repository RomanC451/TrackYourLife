using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises;

internal sealed class ExerciseReadModelConfiguration : IEntityTypeConfiguration<ExerciseReadModel>
{
    public void Configure(EntityTypeBuilder<ExerciseReadModel> builder)
    {
        builder.ToTable(TableNames.Exercise);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasConversion(v => v.Value, v => ExerciseId.Create(v));

        builder
            .Property(e => e.UserId)
            .HasConversion(v => v.Value, v => UserId.Create(v))
            .IsRequired();
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.PictureUrl).IsRequired(false);
        builder.Property(e => e.VideoUrl).IsRequired(false);
        builder.Property(e => e.Description).IsRequired(false);
        builder.Property(e => e.Equipment).IsRequired(false);
        builder.Property(e => e.CreatedOnUtc).IsRequired();
        builder.Property(e => e.ModifiedOnUtc).IsRequired(false);
        builder.Property(e => e.ExerciseSetsJson).HasColumnType("jsonb").IsRequired();

        builder.Ignore(e => e.ExerciseSets);
    }
}
