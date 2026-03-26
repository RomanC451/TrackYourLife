using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubePlaylists;

internal sealed class YoutubePlaylistConfiguration : IEntityTypeConfiguration<YoutubePlaylist>
{
    public void Configure(EntityTypeBuilder<YoutubePlaylist> builder)
    {
        builder.ToTable(TableNames.YoutubePlaylist);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasConversion(v => v.Value, v => YoutubePlaylistId.Create(v));

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);

        builder.Property(e => e.CreatedOnUtc).IsRequired();

        builder.Property(e => e.ModifiedOnUtc);

        builder.HasIndex(e => e.UserId);
    }
}
