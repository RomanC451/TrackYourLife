using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarPlaylists;

internal sealed class GuitarPlaylistReadModelConfiguration
    : IEntityTypeConfiguration<GuitarPlaylistReadModel>
{
    public void Configure(EntityTypeBuilder<GuitarPlaylistReadModel> builder)
    {
        builder.ToTable(TableNames.GuitarPlaylist);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasConversion(v => v.Value, v => GuitarPlaylistId.Create(v));

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.Name).IsRequired().HasMaxLength(GuitarPlaylist.NameMaxLength);

        builder.Property(e => e.CreatedOnUtc).IsRequired();

        builder.Property(e => e.ModifiedOnUtc);
    }
}
