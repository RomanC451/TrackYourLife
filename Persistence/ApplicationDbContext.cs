using Microsoft.EntityFrameworkCore;
using TrackYourLifeDotnet.Domain.Entities;

namespace TrackYourLifeDotnet.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<WeatherForecast> WeatherForecasts { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
        optionsBuilder.UseNpgsql(
            "Server=localhost;Database=track-your-life;Username=postgres;Password=Waryor.001"
        );

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
}
