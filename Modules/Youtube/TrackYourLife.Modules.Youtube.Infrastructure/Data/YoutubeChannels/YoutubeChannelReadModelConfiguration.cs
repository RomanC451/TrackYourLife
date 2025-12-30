using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels;

internal sealed class YoutubeChannelReadModelConfiguration
    : IEntityTypeConfiguration<YoutubeChannelReadModel>
{
    public void Configure(EntityTypeBuilder<YoutubeChannelReadModel> builder)
    {
        builder.ToTable(TableNames.YoutubeChannel);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasConversion(v => v.Value, v => YoutubeChannelId.Create(v));

        builder.Property(e => e.UserId).IsRequired();
        builder.Property(e => e.YoutubeChannelId).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.ThumbnailUrl).IsRequired(false).HasMaxLength(500);
        builder.Property(e => e.Category).IsRequired();
        builder.Property(e => e.CreatedOnUtc).IsRequired();
        builder.Property(e => e.ModifiedOnUtc).IsRequired(false);
    }
}

