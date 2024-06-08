using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Persistence.Constants;

namespace TrackYourLife.Common.Persistence.Configurations.Write
{
    public class SearchedFoodConfiguration : IEntityTypeConfiguration<SearchedFood>
    {
        public void Configure(EntityTypeBuilder<SearchedFood> builder)
        {
            builder.ToTable(TableNames.SearchedFood);

            builder.HasKey(sf => sf.Id);
            builder
                .Property(sf => sf.Id)
                .HasConversion(id => id.Value, value => new SearchedFoodId(value));

            builder.Property(sf => sf.Name).IsRequired();
        }
    }
}
