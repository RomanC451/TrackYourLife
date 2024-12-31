using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Ingredients;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable(TableNames.Ingredient);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity).IsRequired();

        builder.HasOne<Food>().WithMany().HasForeignKey(i => i.FoodId).IsRequired();

        builder.HasOne<ServingSize>().WithMany().HasForeignKey(x => x.ServingSizeId).IsRequired();
    }
}
