using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes;

internal sealed class RecipeReadModelConfiguration : IEntityTypeConfiguration<RecipeReadModel>
{
    public void Configure(EntityTypeBuilder<RecipeReadModel> builder)
    {
        builder.ToTable(TableNames.Recipe);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId);

        builder.ComplexProperty(
            f => f.NutritionalContents,
            nc =>
            {
                nc.ComplexProperty(n => n.Energy);
            }
        );

        builder.HasMany(x => x.Ingredients).WithOne().HasForeignKey("RecipeId");
    }
}
