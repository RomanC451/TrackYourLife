using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings;

internal sealed class TrainingConfiguration : IEntityTypeConfiguration<Training>
{
    public void Configure(EntityTypeBuilder<Training> builder)
    {
        builder.ToTable(TableNames.Training);

        builder.HasKey(t => t.Id);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(t => t.Name).IsRequired();

        builder.Property(t => t.Duration).IsRequired(false);

        builder.Property(t => t.Description).IsRequired(false);

        builder.Property(t => t.CreatedOnUtc).IsRequired();

        builder.Property(t => t.ModifiedOnUtc).IsRequired(false);

        builder.HasMany(t => t.TrainingExercises).WithOne().HasForeignKey(te => te.TrainingId);
    }
}
