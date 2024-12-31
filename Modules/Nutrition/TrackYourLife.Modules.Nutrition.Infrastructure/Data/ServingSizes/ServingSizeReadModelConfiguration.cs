using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes;

internal sealed class ServingSizeReadModelConfiguration
    : IEntityTypeConfiguration<ServingSizeReadModel>
{
    public void Configure(EntityTypeBuilder<ServingSizeReadModel> builder)
    {
        builder.ToTable(TableNames.ServingSize);

        builder.HasKey(f => f.Id);
    }
}
