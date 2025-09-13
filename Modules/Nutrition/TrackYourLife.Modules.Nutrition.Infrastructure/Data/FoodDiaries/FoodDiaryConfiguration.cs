using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodDiaries;

internal sealed class FoodDiaryConfiguration : IEntityTypeConfiguration<FoodDiary>
{
    public void Configure(EntityTypeBuilder<FoodDiary> builder)
    {
        builder.ToTable(TableNames.FoodDiary);

        builder.HasKey(de => de.Id);

        builder.Property(de => de.UserId).IsRequired();

        builder.Property(de => de.Date).IsRequired();

        builder.Property(de => de.MealType).HasConversion<string>().IsRequired();

        builder.Property(x => x.CreatedOnUtc).IsRequired();
        builder.Property(x => x.ModifiedOnUtc).IsRequired(false);

        builder.HasOne<Food>().WithMany().HasForeignKey(de => de.FoodId).IsRequired();

        builder.HasOne<ServingSize>().WithMany().HasForeignKey(de => de.ServingSizeId).IsRequired();
    }
}
