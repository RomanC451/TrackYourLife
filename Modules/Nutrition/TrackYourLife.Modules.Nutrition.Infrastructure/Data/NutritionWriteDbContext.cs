using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Outbox;
using TrackYourLife.SharedLib.Infrastructure.Extensions;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data;

/// <summary>
/// Represents the database context for writing nutrition-related data.
/// </summary>
public sealed class NutritionWriteDbContext(
    DbContextOptions<NutritionWriteDbContext> options,
    IConfiguration? configuration
) : DbContext(options)
{
    public readonly IConfiguration? _configuration = configuration;

    public DbSet<Food> Foods { get; set; }
    public DbSet<FoodDiary> FoodDiaries { get; set; }
    public DbSet<RecipeDiary> RecipeDiaries { get; set; }
    public DbSet<SearchedFood> SearchedFoods { get; set; }
    public DbSet<ServingSize> ServingSizes { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<FoodHistory> FoodHistories { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_configuration != null)
        {
            optionsBuilder
                .UseNpgsql(
                    _configuration.GetConnectionString("Database"),
                    opts =>
                        opts.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "Nutrition")
                )
                .EnableSensitiveDataLogging()
                .AddInterceptors(new ConvertDomainEventsToOutboxMessagesInterceptor());
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Nutrition");
        modelBuilder.ApplyConfigurationsFromAssembly(
            AssemblyReference.Assembly,
            WriteConfigurationsFilter
        );

        modelBuilder.UseStronglyTypedIdsValueConverter();
    }

    private static bool WriteConfigurationsFilter(Type type) =>
        !type.Name?.Contains("ReadModel") ?? false;
}
