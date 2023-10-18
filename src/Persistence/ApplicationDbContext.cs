using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Persistence.Interceptors;

namespace TrackYourLifeDotnet.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public readonly IConfiguration? _configuration;

    public ApplicationDbContext(DbContextOptions options, IConfiguration? configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<UserToken> UserTokens { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")).AddInterceptors(new ConvertDomainEventsToOutboxMessagesInterceptor());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
}
