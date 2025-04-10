using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes;

internal sealed class ServingSizeConfiguration : IEntityTypeConfiguration<ServingSize>
{
    public void Configure(EntityTypeBuilder<ServingSize> builder)
    {
        builder.ToTable(TableNames.ServingSize);

        builder.HasKey(ss => ss.Id);

        builder.HasIndex(ss => ss.ApiId).IsUnique();

        builder.Property(ss => ss.Unit).IsRequired();

        builder.Property(ss => ss.Value).IsRequired();

        builder.Property(ss => ss.NutritionMultiplier).IsRequired();

        builder.Property(ss => ss.ApiId).IsRequired(false);
    }
}
