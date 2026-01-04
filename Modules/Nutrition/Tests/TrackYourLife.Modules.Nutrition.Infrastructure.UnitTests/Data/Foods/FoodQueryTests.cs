using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Foods;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.Foods;

public class FoodQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private FoodQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new FoodQuery(ReadDbContext);
    }

    [Fact]
    public async Task SearchFoodAsync_WhenFoodsExist_ShouldReturnMatchingFoods()
    {
        // Arrange
        var searchTerm = "apple";

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await WriteDbContext.ServingSizes.AddAsync(servingSize);
        await WriteDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food1 = FoodFaker.Generate(
            name: "Apple",
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        var food2 = FoodFaker.Generate(
            name: "Banana",
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        var food3 = FoodFaker.Generate(
            name: "Apple Pie",
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        await WriteDbContext.Foods.AddAsync(food1);
        await WriteDbContext.Foods.AddAsync(food2);
        await WriteDbContext.Foods.AddAsync(food3);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.SearchFoodAsync(searchTerm, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(f => f.Name == "Apple");
            result.Should().Contain(f => f.Name == "Apple Pie");
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task SearchFoodAsync_WhenNoMatchingFoodsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var searchTerm = "nonexistent";

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await WriteDbContext.ServingSizes.AddAsync(servingSize);
        await WriteDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food1 = FoodFaker.Generate(
            name: "Apple",
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        var food2 = FoodFaker.Generate(
            name: "Banana",
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        await WriteDbContext.Foods.AddAsync(food1);
        await WriteDbContext.Foods.AddAsync(food2);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.SearchFoodAsync(searchTerm, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetFoodsPartOfAsync_WhenFoodsExist_ShouldReturnMatchingFoods()
    {
        // Arrange
        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await WriteDbContext.ServingSizes.AddAsync(servingSize);
        await WriteDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
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

        var food3 = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );

        await WriteDbContext.Foods.AddAsync(food1);
        await WriteDbContext.Foods.AddAsync(food2);
        await WriteDbContext.Foods.AddAsync(food3);
        await WriteDbContext.SaveChangesAsync();

        var foodIds = new List<FoodId> { food1.Id, food3.Id };

        try
        {
            // Act
            var result = await _sut.GetFoodsPartOfAsync(foodIds, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(f => f.Id == food1.Id);
            result.Should().Contain(f => f.Id == food3.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetFoodsPartOfAsync_WhenNoMatchingFoodsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var foodIds = new List<FoodId> { FoodId.NewId(), FoodId.NewId() };

        try
        {
            // Act
            var result = await _sut.GetFoodsPartOfAsync(foodIds, CancellationToken.None);

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
