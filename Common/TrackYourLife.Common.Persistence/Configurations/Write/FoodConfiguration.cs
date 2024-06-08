using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Persistence.Constants;

namespace TrackYourLife.Common.Persistence.Configurations.Write;

public class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.ToTable(TableNames.Food);

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasConversion(f => f.Value, f => new FoodId(f));

        builder.HasIndex(f => f.ApiId).IsUnique();
        builder.Property(f => f.ApiId).IsRequired(false);

        builder.Property(f => f.Type).IsRequired();
        builder.Property(f => f.BrandName).HasDefaultValue("");
        builder.Property(f => f.CountryCode).IsRequired();
        builder.Property(f => f.Name).IsRequired();

        builder.OwnsOne(
            f => f.NutritionalContent,
            nc =>
            {
                nc.Property(n => n.Calcium).IsRequired();
                nc.Property(n => n.Carbohydrates).IsRequired();
                nc.Property(n => n.Cholesterol).IsRequired();
                nc.Property(n => n.Fat).IsRequired();
                nc.Property(n => n.Fiber).IsRequired();
                nc.Property(n => n.Iron).IsRequired();
                nc.Property(n => n.MonounsaturatedFat).IsRequired();
                nc.Property(n => n.NetCarbs).IsRequired();
                nc.Property(n => n.PolyunsaturatedFat).IsRequired();
                nc.Property(n => n.Potassium).IsRequired();
                nc.Property(n => n.Protein).IsRequired();
                nc.Property(n => n.SaturatedFat).IsRequired();
                nc.Property(n => n.Sodium).IsRequired();
                nc.Property(n => n.Sugar).IsRequired();
                nc.Property(n => n.TransFat).IsRequired();
                nc.Property(n => n.VitaminA).IsRequired();
                nc.Property(n => n.VitaminC).IsRequired();

                nc.OwnsOne(
                    n => n.Energy,
                    e =>
                    {
                        e.Property(en => en.Unit).IsRequired();
                        e.Property(en => en.Value).IsRequired();
                    }
                );
            }
        );

        builder.HasMany(f => f.FoodServingSizes).WithOne().HasForeignKey(fss => fss.FoodId);
    }
}
