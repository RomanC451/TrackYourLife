using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory;

internal sealed class FoodHistoryReadModelConfiguration
    : IEntityTypeConfiguration<FoodHistoryReadModel>
{
    public void Configure(EntityTypeBuilder<FoodHistoryReadModel> builder)
    {
        builder.ToTable(TableNames.FoodHistory);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId);

        builder.Property(x => x.FoodId);

        builder.Property(x => x.LastUsedAt);

        builder.HasOne<FoodReadModel>().WithMany().HasForeignKey(x => x.FoodId);
    }
}
