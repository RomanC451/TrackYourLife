using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.Modules.Youtube.Infrastructure.Data.Constants;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.DailyEntertainmentCounters;

internal sealed class DailyEntertainmentCountersConfiguration
    : IEntityTypeConfiguration<DailyEntertainmentCounter>
{
    public void Configure(EntityTypeBuilder<DailyEntertainmentCounter> builder)
    {
        builder.ToTable(TableNames.DailyEntertainmentCounter);

        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasConversion(v => v.Value, v => DailyEntertainmentCounterId.Create(v));

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.Date).IsRequired();

        builder.Property(e => e.VideosWatchedCount).IsRequired();

        // Composite unique index on (UserId, Date) - one counter per user per day
        builder.HasIndex(e => new { e.UserId, e.Date }).IsUnique();

        // Index on UserId for faster queries
        builder.HasIndex(e => e.UserId);
    }
}
