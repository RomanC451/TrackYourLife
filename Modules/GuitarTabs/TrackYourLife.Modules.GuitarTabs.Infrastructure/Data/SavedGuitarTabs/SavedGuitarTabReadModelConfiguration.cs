using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.SavedGuitarTabs;

internal sealed class SavedGuitarTabReadModelConfiguration : IEntityTypeConfiguration<SavedGuitarTabReadModel>
{
    public void Configure(EntityTypeBuilder<SavedGuitarTabReadModel> builder)
    {
        builder.ToTable(TableNames.SavedGuitarTab);

        builder.HasKey(tab => tab.Id);

        builder.Property(tab => tab.Id).HasConversion(value => value.Value, value => SavedGuitarTabId.Create(value));

        builder.Property(tab => tab.UserId).IsRequired();

        builder.Property(tab => tab.TabUrl).IsRequired().HasMaxLength(SavedGuitarTab.TabUrlMaxLength);

        builder.Property(tab => tab.SongName).IsRequired().HasMaxLength(SavedGuitarTab.SongNameMaxLength);

        builder.Property(tab => tab.ArtistName).IsRequired().HasMaxLength(SavedGuitarTab.ArtistNameMaxLength);

        builder.Property(tab => tab.Type).IsRequired().HasMaxLength(SavedGuitarTab.TypeMaxLength);

        builder.Property(tab => tab.IsFavorite).IsRequired();

        builder.Property(tab => tab.YoutubeUrl).HasMaxLength(SavedGuitarTab.YoutubeUrlMaxLength);

        builder.Property(tab => tab.CreatedOnUtc).IsRequired();

        builder.Property(tab => tab.ModifiedOnUtc);
    }
}
