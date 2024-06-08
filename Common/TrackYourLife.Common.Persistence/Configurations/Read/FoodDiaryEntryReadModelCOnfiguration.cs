using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Persistence.Constants;

namespace TrackYourLife.Common.Persistence.Configurations.Read;

internal sealed class FoodDiaryEntryReadModelCOnfiguration
    : IEntityTypeConfiguration<FoodDiaryEntryReadModel>
{
    public void Configure(EntityTypeBuilder<FoodDiaryEntryReadModel> builder)
    {
        builder.ToTable(TableNames.FoodDiary);

        builder.HasKey(de => de.Id);

        builder.HasOne(de => de.Food).WithMany();

        builder.HasOne(de => de.ServingSize).WithMany();
    }
}
