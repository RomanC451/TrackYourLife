using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Foods;

internal sealed class FoodReadModelConfiguration : IEntityTypeConfiguration<FoodReadModel>
{
    public void Configure(EntityTypeBuilder<FoodReadModel> builder)
    {
        builder.ToTable(TableNames.Food);

        builder.HasKey(f => f.Id);

        builder.HasMany(f => f.FoodServingSizes).WithOne();

        builder.ComplexProperty(
            f => f.NutritionalContents,
            nc =>
            {
                nc.ComplexProperty(nc => nc.Energy);
            }
        );
    }
}
