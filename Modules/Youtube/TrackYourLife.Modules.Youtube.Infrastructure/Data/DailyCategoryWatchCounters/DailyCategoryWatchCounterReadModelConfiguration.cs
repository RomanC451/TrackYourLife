using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.DailyCategoryWatchCounters;

internal sealed class DailyCategoryWatchCounterReadModelConfiguration
    : IEntityTypeConfiguration<DailyCategoryWatchCounterReadModel>
{
    public void Configure(EntityTypeBuilder<DailyCategoryWatchCounterReadModel> builder)
    {
        builder.ToTable(TableNames.DailyCategoryWatchCounter);

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasConversion(v => v.Value, v => DailyCategoryWatchCounterId.Create(v));

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.Date).IsRequired();

        builder
            .Property(e => e.YoutubeCategoryId)
            .HasConversion(v => v.Value, v => YoutubeCategoryId.Create(v))
            .IsRequired();

        builder.Property(e => e.VideosWatchedCount).IsRequired();
    }
}
