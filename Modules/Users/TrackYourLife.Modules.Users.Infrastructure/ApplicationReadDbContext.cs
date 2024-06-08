
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Users.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Infrastructure;

internal sealed class ApplicationReadDbContext(DbContextOptions<ApplicationReadDbContext> options, IConfiguration? configuration) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<UserReadModel> Users { get; set; }

    public DbSet<UserGoalReadModel> UserGoals { get; set; }


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
