using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;
using TrackYourLifeDotnet.Persistence.Constants;

namespace TrackYourLifeDotnet.Persistence.Configurations;

public class FoodServingSizeConfiguration : IEntityTypeConfiguration<FoodServingSize>
{
    public void Configure(EntityTypeBuilder<FoodServingSize> builder)
    {
        builder.ToTable(TableNames.FoodServingSize);

        builder.HasKey(fss => new { fss.FoodId, fss.ServingSizeId });

        builder.Property(fss => fss.FoodId).HasConversion(f => f.Value, f => new FoodId(f));

        builder
            .Property(fss => fss.ServingSizeId)
            .HasConversion(ss => ss.Value, ss => new ServingSizeId(ss));

        builder.HasOne<Food>().WithMany(f => f.FoodServingSizes).HasForeignKey(fss => fss.FoodId);

        builder.HasOne(fss => fss.ServingSize).WithMany().HasForeignKey(fss => fss.ServingSizeId);
    }
}
