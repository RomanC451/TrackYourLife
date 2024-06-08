using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Common.Persistence.Interceptors;
using TrackYourLife.Modules.Users.Domain.UserGoals;
using TrackYourLife.Modules.Users.Domain.Users;

namespace TrackYourLife.Modules.Users.Infrastructure;

public sealed class ApplicationWriteDbContext(DbContextOptions<ApplicationWriteDbContext> options, IConfiguration? configuration) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<User> Users { get; set; }

    public DbSet<UserToken> UserTokens { get; set; }

    public DbSet<UserGoal> UserGoals { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder
                .UseNpgsql(_configuration.GetConnectionString("Database"))
                .EnableSensitiveDataLogging()
                .AddInterceptors(new ConvertDomainEventsToOutboxMessagesInterceptor());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly, WriteConfigurationsFilter);
    }

    private static bool WriteConfigurationsFilter(Type type) => type.FullName?.Contains("Configurations.Write") ?? false;
}
