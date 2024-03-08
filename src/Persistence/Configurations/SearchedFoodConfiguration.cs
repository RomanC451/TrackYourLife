using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;
using TrackYourLifeDotnet.Persistence.Constants;

namespace TrackYourLifeDotnet.Persistence.Configurations
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
