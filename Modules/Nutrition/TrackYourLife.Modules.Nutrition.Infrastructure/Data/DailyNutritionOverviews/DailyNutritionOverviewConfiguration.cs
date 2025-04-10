using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews;

internal sealed class DailyNutritionOverviewConfiguration
    : IEntityTypeConfiguration<DailyNutritionOverview>
{
    public void Configure(EntityTypeBuilder<DailyNutritionOverview> builder)
    {
        builder.ToTable(TableNames.DailyNutritionOverview);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();

        builder.Property(x => x.Date).IsRequired();

        builder.ComplexProperty(
            x => x.NutritionalContent,
            nc =>
            {
                nc.ComplexProperty(n => n.Energy);
            }
        );

        builder.Property(x => x.CaloriesGoal).IsRequired();

        builder.Property(x => x.CarbohydratesGoal).IsRequired();

        builder.Property(x => x.FatGoal).IsRequired();

        builder.Property(x => x.ProteinGoal).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Date }).IsUnique();
    }
}
