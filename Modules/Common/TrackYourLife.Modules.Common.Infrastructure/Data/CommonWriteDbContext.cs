using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Common.Domain.Features.Cookies;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Common.Infrastructure.Data;

/// <summary>
/// Represents the database context for writing common related data.
/// </summary>
public sealed class CommonWriteDbContext(
    DbContextOptions<CommonWriteDbContext> options,
    IConfiguration configuration
) : DbContext(options)
{
    public DbSet<Cookie> Cookies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (configuration is null)
        {
            return;
        }
        optionsBuilder
            .UseNpgsql(
                configuration.GetConnectionString("Database"),
                opts => opts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Common")
            )
            .EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Common");
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            WriteConfigurationsFilter
        );
        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool WriteConfigurationsFilter(Type type) =>
        !type.Name?.Contains("ReadModel") ?? false;
}
