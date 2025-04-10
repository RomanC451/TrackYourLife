using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Foods;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.Foods;

[Collection("NutritionRepositoryTests")]
public class FoodRepositoryTests : BaseRepositoryTests
{
    private FoodRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new FoodRepository(_writeDbContext!);
    }

    [Fact]
    public async Task GetByApiIdAsync_WhenFoodExists_ShouldReturnFood()
    {
        // Arrange
        var apiId = 12345L;

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food = FoodFaker.Generate(
            apiId: apiId,
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        await _writeDbContext.Foods.AddAsync(food);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByApiIdAsync(apiId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(food.Id);
            result.ApiId.Should().Be(apiId);
            result.FoodServingSizes.Should().HaveCount(1);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByApiIdAsync_WhenFoodDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var apiId = 99999L;

        try
        {
            // Act
            var result = await _sut.GetByApiIdAsync(apiId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetWhereApiIdPartOfListAsync_WhenFoodsExist_ShouldReturnMatchingFoods()
    {
        // Arrange
        var apiIds = new List<long> { 11111L, 33333L };

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food1 = FoodFaker.Generate(
            apiId: apiIds[0],
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        var food2 = FoodFaker.Generate(
            apiId: 22222L,
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        var food3 = FoodFaker.Generate(
            apiId: apiIds[1],
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        await _writeDbContext.Foods.AddAsync(food1);
        await _writeDbContext.Foods.AddAsync(food2);
        await _writeDbContext.Foods.AddAsync(food3);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetWhereApiIdPartOfListAsync(apiIds, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(f => f.ApiId == apiIds[0]);
            result.Should().Contain(f => f.ApiId == apiIds[1]);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetWhereApiIdPartOfListAsync_WhenNoMatchingFoodsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var apiIds = new List<long> { 99999L, 88888L };

        try
        {
            // Act
            var result = await _sut.GetWhereApiIdPartOfListAsync(apiIds, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
