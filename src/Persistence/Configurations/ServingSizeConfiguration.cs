using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;
using TrackYourLifeDotnet.Persistence.Constants;

namespace TrackYourLifeDotnet.Persistence.Configurations;

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
