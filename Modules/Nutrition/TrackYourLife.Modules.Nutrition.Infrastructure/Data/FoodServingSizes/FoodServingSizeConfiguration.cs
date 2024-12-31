using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodServingSizes;

public class FoodServingSizeConfiguration : IEntityTypeConfiguration<FoodServingSize>
{
    public void Configure(EntityTypeBuilder<FoodServingSize> builder)
    {
        builder.ToTable(TableNames.FoodServingSize);

        builder.HasKey(fss => new { fss.FoodId, fss.ServingSizeId });

        builder.Property(fss => fss.FoodId).IsRequired();

        builder.Property(fss => fss.ServingSizeId).IsRequired();

        builder.Property(fss => fss.Index).IsRequired();

        builder
            .HasOne<Food>()
            .WithMany(f => f.FoodServingSizes)
            .HasForeignKey(fss => fss.FoodId)
            .IsRequired();

        builder
            .HasOne<ServingSize>()
            .WithMany()
            .HasForeignKey(fss => fss.ServingSizeId)
            .IsRequired();
    }
}
