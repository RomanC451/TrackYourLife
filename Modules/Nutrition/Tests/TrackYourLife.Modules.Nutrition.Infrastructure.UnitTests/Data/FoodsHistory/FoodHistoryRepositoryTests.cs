using System.Reflection;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.FoodsHistory;

public class FoodHistoryRepositoryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private FoodHistoryRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new FoodHistoryRepository(WriteDbContext);
    }

    [Fact]
    public async Task GetByUserAndFoodAsync_WhenHistoryExists_ShouldReturnHistory()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await WriteDbContext.ServingSizes.AddAsync(servingSize);
        await WriteDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, foodId, servingSize),
            }
        );

        await WriteDbContext.Foods.AddAsync(food);
        await WriteDbContext.SaveChangesAsync();

        // Create and save FoodHistory
        var foodHistory = FoodHistory.Create(
            FoodHistoryId.NewId(),
            userId,
            foodId,
            servingSize.Id,
            1.0f
        ).Value;
        await WriteDbContext.FoodHistories.AddAsync(foodHistory);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByUserAndFoodAsync(userId, foodId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(foodHistory.Id);
            result.UserId.Should().Be(userId);
            result.FoodId.Should().Be(foodId);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserAndFoodAsync_WhenHistoryDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByUserAndFoodAsync(userId, foodId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetUserHistoryCountAsync_WhenHistoriesExist_ShouldReturnCount()
    {
        // Arrange
        var userId = UserId.NewId();

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await WriteDbContext.ServingSizes.AddAsync(servingSize);
        await WriteDbContext.SaveChangesAsync();

        // Create and save Foods with FoodServingSizes
        var food1 = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        var food2 = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        await WriteDbContext.Foods.AddRangeAsync(food1, food2);
        await WriteDbContext.SaveChangesAsync();

        // Create and save FoodHistories
        var foodHistory1 = FoodHistory.Create(
            FoodHistoryId.NewId(),
            userId,
            food1.Id,
            servingSize.Id,
            1.0f
        ).Value;
        var foodHistory2 = FoodHistory.Create(
            FoodHistoryId.NewId(),
            userId,
            food2.Id,
            servingSize.Id,
            2.0f
        ).Value;

        await WriteDbContext.FoodHistories.AddRangeAsync(foodHistory1, foodHistory2);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetUserHistoryCountAsync(userId, CancellationToken.None);

            // Assert
            result.Should().Be(2);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetUserHistoryCountAsync_WhenNoHistoriesExist_ShouldReturnZero()
    {
        // Arrange
        var userId = UserId.NewId();

        try
        {
            // Act
            var result = await _sut.GetUserHistoryCountAsync(userId, CancellationToken.None);

            // Assert
            result.Should().Be(0);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetOldestByUserAsync_WhenHistoriesExist_ShouldReturnOldestHistory()
    {
        // Arrange
        var userId = UserId.NewId();

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await WriteDbContext.ServingSizes.AddAsync(servingSize);
        await WriteDbContext.SaveChangesAsync();

        // Create and save Foods with FoodServingSizes
        var food1 = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        var food2 = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        await WriteDbContext.Foods.AddRangeAsync(food1, food2);
        await WriteDbContext.SaveChangesAsync();

        // Create and save FoodHistories with different dates
        var oldestDate = DateTime.UtcNow.AddDays(-2);
        var newerDate = DateTime.UtcNow.AddDays(-1);

        var foodHistory1 = FoodHistory.Create(
            FoodHistoryId.NewId(),
            userId,
            food1.Id,
            servingSize.Id,
            1.0f
        ).Value;
        var foodHistory2 = FoodHistory.Create(
            FoodHistoryId.NewId(),
            userId,
            food2.Id,
            servingSize.Id,
            2.0f
        ).Value;

        // Use reflection to set the LastUsedAt property
        var lastUsedAtProperty = typeof(FoodHistory).GetProperty(
            "LastUsedAt",
            BindingFlags.Public | BindingFlags.Instance
        );
        lastUsedAtProperty?.SetValue(foodHistory1, oldestDate);
        lastUsedAtProperty?.SetValue(foodHistory2, newerDate);

        await WriteDbContext.FoodHistories.AddRangeAsync(foodHistory1, foodHistory2);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetOldestByUserAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(foodHistory1.Id);
            result.LastUsedAt.Should().Be(oldestDate);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetOldestByUserAsync_WhenNoHistoriesExist_ShouldReturnNull()
    {
        // Arrange
        var userId = UserId.NewId();

        try
        {
            // Act
            var result = await _sut.GetOldestByUserAsync(userId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
