using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.WatchedVideos;

internal sealed class WatchedVideoConfiguration : IEntityTypeConfiguration<WatchedVideo>
{
    public void Configure(EntityTypeBuilder<WatchedVideo> builder)
    {
        builder.ToTable(TableNames.WatchedVideo);

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasConversion(v => v.Value, v => WatchedVideoId.Create(v));

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.VideoId).IsRequired().HasMaxLength(50);

        builder.Property(e => e.ChannelId).IsRequired().HasMaxLength(50);

        builder.Property(e => e.WatchedAtUtc).IsRequired();

        // Composite unique index on (UserId, VideoId) - one watched entry per video per user
        builder.HasIndex(e => new { e.UserId, e.VideoId }).IsUnique();

        // Index on UserId for faster queries
        builder.HasIndex(e => e.UserId);
    }
}
