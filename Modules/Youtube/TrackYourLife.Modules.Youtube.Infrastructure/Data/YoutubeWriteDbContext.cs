using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.Extensions;
using TrackYourLife.SharedLib.Infrastructure.Outbox;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data;

/// <summary>
/// Represents the database context for writing YouTube-related data.
/// </summary>
public sealed class YoutubeWriteDbContext(
    DbContextOptions<YoutubeWriteDbContext> options,
    IConfiguration? configuration
) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<YoutubeChannel> YoutubeChannels { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder
                .UseNpgsql(
                    _configuration.GetConnectionString("Database"),
                    opts =>
                        opts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Youtube")
                )
                .EnableSensitiveDataLogging()
                .AddInterceptors(new ConvertDomainEventsToOutboxMessagesInterceptor());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Youtube");
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            WriteConfigurationsFilter
        );

        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool WriteConfigurationsFilter(Type type) =>
        !type.Name?.Contains("ReadModel") ?? false;
}

