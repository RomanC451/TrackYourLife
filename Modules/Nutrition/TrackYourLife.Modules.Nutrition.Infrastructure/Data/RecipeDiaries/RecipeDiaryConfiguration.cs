using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries;

internal sealed class RecipeDiaryConfiguration : IEntityTypeConfiguration<RecipeDiary>
{
    public void Configure(EntityTypeBuilder<RecipeDiary> builder)
    {
        builder.ToTable(TableNames.RecipeDiary);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.ServingSizeId).IsRequired();

        builder.Property(x => x.Date).IsRequired();

        builder.Property(x => x.MealType).HasConversion<string>().IsRequired();

        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.ModifiedOnUtc).IsRequired(false);

        builder.HasOne<Recipe>().WithMany().HasForeignKey(x => x.RecipeId).IsRequired();
    }
}
