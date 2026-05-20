using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeCategories;

internal sealed class YoutubeCategoryReadModelConfiguration
    : IEntityTypeConfiguration<YoutubeCategoryReadModel>
{
    public void Configure(EntityTypeBuilder<YoutubeCategoryReadModel> builder)
    {
        builder.ToTable(TableNames.YoutubeCategory);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasConversion(v => v.Value, v => YoutubeCategoryId.Create(v));

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.Name).IsRequired().HasMaxLength(YoutubeCategory.MaxNameLength);

        builder.Property(e => e.MaxVideosPerDay).IsRequired();

        builder.Property(e => e.DisplayOrder).IsRequired();

        builder.Property(e => e.CreatedOnUtc).IsRequired();

        builder.Property(e => e.ModifiedOnUtc);
    }
}
