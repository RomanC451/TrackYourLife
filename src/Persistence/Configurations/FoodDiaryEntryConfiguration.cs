using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLifeDotnet.Domain.FoodDiaries;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Domain.Users;

using TrackYourLifeDotnet.Persistence.Constants;

namespace TrackYourLifeDotnet.Persistence.Configurations;

public class FoodDiaryEntryConfiguration : IEntityTypeConfiguration<FoodDiaryEntry>
{
    public void Configure(EntityTypeBuilder<FoodDiaryEntry> builder)
    {
        builder.ToTable(TableNames.FoodDiary);

        builder.HasKey(de => de.Id);
        builder
            .Property(de => de.Id)
            .HasConversion(id => id.Value, value => new FoodDiaryEntryId(value));

        builder.Property(de => de.UserId).HasConversion(id => id.Value, value => new UserId(value));

        builder.Property(de => de.Quantity);
        builder.Property(de => de.Date);
        builder
            .Property(de => de.MealType)
            .HasConversion(
                token => token.ToString(),
                value => (MealTypes)Enum.Parse(typeof(MealTypes), value)
            );
        ;

        builder.HasOne<User>().WithMany().HasForeignKey(de => de.UserId);

        builder.HasOne(de => de.Food).WithMany();

        builder.HasOne(de => de.ServingSize).WithMany();
    }
}
