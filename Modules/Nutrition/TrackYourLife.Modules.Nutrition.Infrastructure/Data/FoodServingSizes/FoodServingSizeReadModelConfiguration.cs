using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodServingSizes;

public class FoodServingSizeReadModelConfiguration
    : IEntityTypeConfiguration<FoodServingSizeReadModel>
{
    public void Configure(EntityTypeBuilder<FoodServingSizeReadModel> builder)
    {
        builder.ToTable(TableNames.FoodServingSize);

        builder.HasKey(fss => new { fss.FoodId, fss.ServingSizeId });

        builder.Property(fss => fss.FoodId);

        builder.Property(fss => fss.ServingSizeId);

        builder
            .HasOne<FoodReadModel>()
            .WithMany(f => f.FoodServingSizes)
            .HasForeignKey(fss => fss.FoodId);

        builder.HasOne(fss => fss.ServingSize).WithMany().HasForeignKey(fss => fss.ServingSizeId);
    }
}
