using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory;

public class FoodHistoryConfiguration : IEntityTypeConfiguration<FoodHistory>
{
    public void Configure(EntityTypeBuilder<FoodHistory> builder)
    {
        builder.ToTable(TableNames.FoodHistory);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.FoodId).IsRequired();
        builder.Property(x => x.LastUsedAt).IsRequired();

        builder.HasOne<Food>()
            .WithMany()
            .HasForeignKey(x => x.FoodId)
            .IsRequired();

        builder.HasIndex(x => new { x.UserId, x.FoodId }).IsUnique();
        builder.HasIndex(x => x.LastUsedAt);
    }
}
