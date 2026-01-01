using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data;

/// <summary>
/// Represents the database context for reading YouTube-related data.
/// </summary>
public sealed class YoutubeReadDbContext(
    DbContextOptions<YoutubeReadDbContext> options,
    IConfiguration? configuration
) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<YoutubeChannelReadModel> YoutubeChannels { get; set; }
    public DbSet<YoutubeSettingReadModel> YoutubeSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder
                .UseNpgsql(_configuration.GetConnectionString("Database"))
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Youtube");

        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            ReadConfigurationsFilter
        );

        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool ReadConfigurationsFilter(Type type) =>
        type.Name?.Contains("ReadModel") ?? false;
}
