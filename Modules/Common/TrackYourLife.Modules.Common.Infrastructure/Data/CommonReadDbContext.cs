using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Common.Infrastructure.Data;

/// <summary>
/// Represents the database context for reading common related data.
/// </summary>
internal sealed class CommonReadDbContext(
    DbContextOptions<CommonReadDbContext> options,
    IConfiguration configuration
) : DbContext(options)
{
    public DbSet<CookieReadModel> Cookies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (configuration is null)
        {
            return;
        }
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Database"))
            .EnableSensitiveDataLogging()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Common");
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            ReadConfigurationsFilter
        );

        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool ReadConfigurationsFilter(Type type) =>
        type.Name?.Contains("ReadModel") ?? false;
}
