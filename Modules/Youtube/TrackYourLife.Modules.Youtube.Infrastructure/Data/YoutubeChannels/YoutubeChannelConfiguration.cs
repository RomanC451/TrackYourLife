using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeChannels;

internal sealed class YoutubeChannelConfiguration : IEntityTypeConfiguration<YoutubeChannel>
{
    public void Configure(EntityTypeBuilder<YoutubeChannel> builder)
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

        builder.Property(e => e.ModifiedOnUtc);

        // Create index for faster queries
        builder.HasIndex(e => new { e.UserId, e.YoutubeChannelId }).IsUnique();
        builder.HasIndex(e => new { e.UserId, e.Category });
    }
}

