using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.Users;
using TrackYourLife.Common.Domain.Users;
using TrackYourLife.Common.Persistence.Constants;

namespace TrackYourLife.Common.Persistence.Configurations.Write;

public class FoodDiaryEntryConfiguration : IEntityTypeConfiguration<FoodDiaryEntry>
{
    public void Configure(EntityTypeBuilder<FoodDiaryEntry> builder)
    {
        builder.ToTable(TableNames.FoodDiary);

        builder.HasKey(de => de.Id);
        builder
            .Property(de => de.Id)
            .HasConversion(id => id.Value, value => FoodDiaryEntryId.Create(value));

        builder.Property(de => de.UserId).HasConversion(id => id.Value, value => new UserId(value));

        builder
            .Property(de => de.MealType)
            .HasConversion(
                token => token.ToString(),
                value => (MealTypes)Enum.Parse(typeof(MealTypes), value)
            );
        ;


        builder.HasOne(de => de.Food).WithMany();

        builder.HasOne(de => de.ServingSize).WithMany();
    }
}
