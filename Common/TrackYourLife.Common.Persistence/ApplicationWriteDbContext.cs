using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Persistence.Interceptors;
using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Domain.OutboxMessages;
using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Persistence;

public sealed class ApplicationWriteDbContext(DbContextOptions<ApplicationWriteDbContext> options, IConfiguration? configuration) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<Food> Foods { get; set; }

    public DbSet<ServingSize> ServingSizes { get; set; }

    public DbSet<FoodServingSize> FoodServingSizes { get; set; }

    public DbSet<SearchedFood> SearchedFood { get; set; }

    public DbSet<FoodDiaryEntry> FoodDiaries { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

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
