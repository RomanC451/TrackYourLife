using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Persistence.Interceptors;
using TrackYourLifeDotnet.Domain.FoodDiaries;

namespace TrackYourLifeDotnet.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public readonly IConfiguration? _configuration;

    public ApplicationDbContext(DbContextOptions options, IConfiguration? configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<User> Users { get; set; }

    public DbSet<UserToken> UserTokens { get; set; }

    public DbSet<Food> Foods { get; set; }

    public DbSet<ServingSize> ServingSizes { get; set; }

    public DbSet<FoodServingSize> FoodServingSizes { get; set; }

    public DbSet<SearchedFood> SearchedFood { get; set; }

    public DbSet<FoodDiaryEntry> FoodDiaries { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder
                .UseNpgsql(_configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging()
                .AddInterceptors(new ConvertDomainEventsToOutboxMessagesInterceptor());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
}
