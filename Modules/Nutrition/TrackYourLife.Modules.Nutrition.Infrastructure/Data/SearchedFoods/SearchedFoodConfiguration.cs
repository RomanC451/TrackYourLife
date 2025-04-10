using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.SearchedFoods
{
    internal sealed class SearchedFoodConfiguration : IEntityTypeConfiguration<SearchedFood>
    {
        public void Configure(EntityTypeBuilder<SearchedFood> builder)
        {
            builder.ToTable(TableNames.SearchedFood);

            builder.HasKey(sf => sf.Id);

            builder.Property(sf => sf.Name).IsRequired();
        }
    }
}
