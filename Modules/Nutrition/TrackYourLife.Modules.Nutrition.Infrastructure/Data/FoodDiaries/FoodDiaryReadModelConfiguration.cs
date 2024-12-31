using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodDiaries;

internal sealed class FoodDiaryReadModelConfiguration : IEntityTypeConfiguration<FoodDiaryReadModel>
{
    public void Configure(EntityTypeBuilder<FoodDiaryReadModel> builder)
    {
        builder.ToTable(TableNames.FoodDiary);

        builder.HasKey(de => de.Id);

        builder.Property(de => de.UserId);

        builder.Property(de => de.MealType).HasConversion<string>();

        builder.HasOne(de => de.Food).WithMany();

        builder.HasOne(de => de.ServingSize).WithMany();
    }
}
