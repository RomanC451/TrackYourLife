using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Ingredients;

internal sealed class IngredientReadModelConfiguration
    : IEntityTypeConfiguration<IngredientReadModel>
{
    public void Configure(EntityTypeBuilder<IngredientReadModel> builder)
    {
        builder.ToTable(TableNames.Ingredient);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity).IsRequired();

        builder.HasOne(x => x.Food).WithMany().IsRequired();

        builder.HasOne(x => x.ServingSize).WithMany().IsRequired();
    }
}
