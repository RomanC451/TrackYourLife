using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries;

internal sealed class RecipeDiaryReadModelConfiguration
    : IEntityTypeConfiguration<RecipeDiaryReadModel>
{
    public void Configure(EntityTypeBuilder<RecipeDiaryReadModel> builder)
    {
        builder.ToTable(TableNames.RecipeDiary);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId);

        builder.Property(x => x.MealType).HasConversion<string>();

        builder.HasOne(x => x.Recipe).WithMany();
    }
}
