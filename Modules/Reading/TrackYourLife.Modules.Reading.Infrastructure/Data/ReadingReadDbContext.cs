using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data;

public sealed class ReadingReadDbContext(
    DbContextOptions<ReadingReadDbContext> options,
    IConfiguration? configuration
) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<BookReadModel> Books { get; set; } = null!;
    public DbSet<ReadingSessionReadModel> ReadingSessions { get; set; } = null!;
    public DbSet<ReadingSessionNoteReadModel> ReadingSessionNotes { get; set; } = null!;

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
        modelBuilder.HasDefaultSchema("Reading");
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            ReadConfigurationsFilter
        );
        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool ReadConfigurationsFilter(Type type) =>
        type.Name?.Contains("ReadModel") ?? false;
}
