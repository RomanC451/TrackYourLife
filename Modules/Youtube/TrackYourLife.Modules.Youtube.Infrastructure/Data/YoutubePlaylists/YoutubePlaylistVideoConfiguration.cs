using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubePlaylists;

internal sealed class YoutubePlaylistVideoConfiguration : IEntityTypeConfiguration<YoutubePlaylistVideo>
{
    public void Configure(EntityTypeBuilder<YoutubePlaylistVideo> builder)
    {
        builder.ToTable(TableNames.YoutubePlaylistVideo);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasConversion(v => v.Value, v => YoutubePlaylistVideoId.Create(v));

        builder.Property(e => e.YoutubePlaylistId).IsRequired();

        builder
            .Property(e => e.YoutubeId)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("VideoId");

        builder.Property(e => e.AddedOnUtc).IsRequired();

        builder
            .HasOne<YoutubePlaylist>()
            .WithMany()
            .HasForeignKey(e => e.YoutubePlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.YoutubePlaylistId, e.YoutubeId }).IsUnique();
    }
}
