using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Foods;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable(TableNames.Food);

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Type).IsRequired();

        builder.Property(f => f.BrandName).HasDefaultValue("").IsRequired();

        builder.Property(f => f.CountryCode).IsRequired();

        builder.Property(f => f.Name).IsRequired();

        builder.HasIndex(f => f.ApiId).IsUnique();

        builder.ComplexProperty(
            f => f.NutritionalContents,
            nc =>
            {
                nc.ComplexProperty(n => n.Energy);
            }
        );

        builder
            .HasMany(f => f.FoodServingSizes)
            .WithOne()
            .HasForeignKey(fss => fss.FoodId)
            .IsRequired();

        builder
            .HasGeneratedTsVectorColumn(
                f => f.SearchVector,
                "english",
                f => new { f.Name, f.BrandName }
            )
            .HasIndex(f => f.SearchVector)
            .HasMethod("GIN");
    }
}
