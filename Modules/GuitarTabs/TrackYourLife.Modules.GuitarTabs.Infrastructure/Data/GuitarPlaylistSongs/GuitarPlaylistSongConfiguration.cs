using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarPlaylistSongs;

internal sealed class GuitarPlaylistSongConfiguration : IEntityTypeConfiguration<GuitarPlaylistSong>
{
    public void Configure(EntityTypeBuilder<GuitarPlaylistSong> builder)
    {
        builder.ToTable(TableNames.GuitarPlaylistSong);

        builder.HasKey(song => song.Id);

        builder
            .Property(song => song.Id)
            .HasConversion(value => value.Value, value => GuitarPlaylistSongId.Create(value));

        builder.Property(song => song.GuitarPlaylistId).IsRequired();

        builder.Property(song => song.SavedGuitarTabId).IsRequired();

        builder.Property(song => song.AddedOnUtc).IsRequired();

        builder
            .HasOne<GuitarPlaylist>()
            .WithMany()
            .HasForeignKey(song => song.GuitarPlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<SavedGuitarTab>()
            .WithMany()
            .HasForeignKey(song => song.SavedGuitarTabId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(song => new { song.GuitarPlaylistId, song.SavedGuitarTabId }).IsUnique();
    }
}
