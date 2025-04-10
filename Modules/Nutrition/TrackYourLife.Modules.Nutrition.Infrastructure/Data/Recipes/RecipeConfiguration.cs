using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes;

internal sealed class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable(TableNames.Recipe);
        builder.HasKey(x => x.Id);

        builder.Property(token => token.UserId).IsRequired();

        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

        builder.Property(x => x.Portions).IsRequired();

        builder.ComplexProperty(
            f => f.NutritionalContents,
            nc =>
            {
                nc.ComplexProperty(n => n.Energy);
            }
        );

        builder
            .HasMany(x => x.Ingredients)
            .WithOne()
            .HasForeignKey("RecipeId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Property(e => e.Xmin)
            .HasColumnName("xmin")
            .HasColumnType("xid") // PostgreSQL's type for transaction IDs
            .IsConcurrencyToken(); // Mark this as a concurrency token
    }
}
