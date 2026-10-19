using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarPlaylistSongs;

internal sealed class GuitarPlaylistSongReadModelConfiguration
    : IEntityTypeConfiguration<GuitarPlaylistSongReadModel>
{
    public void Configure(EntityTypeBuilder<GuitarPlaylistSongReadModel> builder)
    {
        builder.ToTable(TableNames.GuitarPlaylistSong);

        builder.HasKey(song => song.Id);

        builder
            .Property(song => song.Id)
            .HasConversion(value => value.Value, value => GuitarPlaylistSongId.Create(value));

        builder.Property(song => song.GuitarPlaylistId).IsRequired();

        builder.Property(song => song.SavedGuitarTabId).IsRequired();

        builder.Property(song => song.AddedOnUtc).IsRequired();
    }
}
