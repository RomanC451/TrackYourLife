using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Common.Domain.FoodDiaries;

namespace TrackYourLife.Common.Persistence;

internal sealed class ApplicationReadDbContext(DbContextOptions<ApplicationReadDbContext> options, IConfiguration? configuration) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<FoodDiaryEntryReadModel> FoodDiaries { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly, ReadConfigurationsFilter);

    private static bool ReadConfigurationsFilter(Type type) => type.FullName?.Contains("Configurations.Read") ?? false;
}
