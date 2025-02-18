using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews;

internal sealed class DailyNutritionOverviewReadModelConfiguration
    : IEntityTypeConfiguration<DailyNutritionOverviewReadModel>
{
    public void Configure(EntityTypeBuilder<DailyNutritionOverviewReadModel> builder)
    {
        builder.ToTable(TableNames.DailyNutritionOverview);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId);

        builder.ComplexProperty(
            f => f.NutritionalContent,
            nc =>
            {
                nc.ComplexProperty(n => n.Energy);
            }
        );
    }
}
