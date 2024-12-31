using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Domain.Users;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Users.Infrastructure.Data;

internal sealed class UsersReadDbContext(
    DbContextOptions<UsersReadDbContext> options,
    IConfiguration? configuration
) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<UserReadModel> Users { get; set; }
    public DbSet<TokenReadModel> Tokens { get; set; }
    public DbSet<GoalReadModel> UserGoals { get; set; }

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
        modelBuilder.HasDefaultSchema("Users");
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            ReadConfigurationsFilter
        );
        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool ReadConfigurationsFilter(Type type) =>
        type.Name?.Contains("ReadModel") ?? false;
}
