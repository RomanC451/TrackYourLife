using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings;

internal sealed class TrainingReadModelConfiguration : IEntityTypeConfiguration<TrainingReadModel>
{
    public void Configure(EntityTypeBuilder<TrainingReadModel> builder)
    {
        builder.ToTable(TableNames.Training);

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId);
        builder.Property(t => t.Name);
        builder.Property(t => t.Duration);
        builder.Property(t => t.Description);
        builder.Property(t => t.CreatedOnUtc);
        builder.Property(t => t.ModifiedOnUtc);

        builder.HasMany(t => t.TrainingExercises).WithOne();
    }
}
