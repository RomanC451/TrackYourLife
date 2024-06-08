using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Common.Domain.ServingSizes;
using TrackYourLife.Common.Persistence.Constants;

namespace TrackYourLife.Common.Persistence.Configurations.Write;

public class ServingSizeConfiguration : IEntityTypeConfiguration<ServingSize>
{
    public void Configure(EntityTypeBuilder<ServingSize> builder)
    {
        builder.ToTable(TableNames.ServingSize);

        builder.HasKey(ss => ss.Id);
        builder.Property(ss => ss.Id).HasConversion(ss => ss.Value, ss => new ServingSizeId(ss));

        builder.HasIndex(ss => ss.ApiId).IsUnique();
        builder.Property(ss => ss.ApiId).IsRequired(false);

        builder.Property(ss => ss.NutritionMultiplier).IsRequired();
        builder.Property(ss => ss.Unit).IsRequired();
        builder.Property(ss => ss.Value).IsRequired();
    }
}
