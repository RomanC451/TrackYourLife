using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeSettings;

internal sealed class YoutubeSettingsReadModelConfiguration
    : IEntityTypeConfiguration<YoutubeSettingReadModel>
{
    public void Configure(EntityTypeBuilder<YoutubeSettingReadModel> builder)
    {
        builder.ToTable(TableNames.YoutubeSettings);

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).HasConversion(v => v.Value, v => YoutubeSettingsId.Create(v));

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.MaxEntertainmentVideosPerDay).IsRequired();

        builder.Property(e => e.SettingsChangeFrequency).IsRequired();

        builder.Property(e => e.DaysBetweenChanges);

        builder.Property(e => e.LastSettingsChangeUtc);

        builder.Property(e => e.SpecificDayOfWeek);

        builder.Property(e => e.SpecificDayOfMonth);

        builder.Property(e => e.CreatedOnUtc).IsRequired();

        builder.Property(e => e.ModifiedOnUtc);
    }
}
