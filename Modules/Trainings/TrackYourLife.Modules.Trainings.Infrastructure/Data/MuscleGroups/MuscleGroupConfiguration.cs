using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Trainings.Domain.Features.MuscleGroups;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Trainings.Infrastructure.Data.MuscleGroups;

internal sealed class MuscleGroupConfiguration : IEntityTypeConfiguration<MuscleGroup>
{
    public void Configure(EntityTypeBuilder<MuscleGroup> builder)
    {
        builder.ToTable(TableNames.MuscleGroup);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name).IsRequired();

        builder.Property(e => e.ParentMuscleGroupId).IsRequired(false);
    }
}
