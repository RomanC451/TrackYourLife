using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodDiaries;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.FoodDiaries;

[Collection("NutritionRepositoryTests")]
public class FoodDiaryRepositoryTests : BaseRepositoryTests
{
    private FoodDiaryRepository? _sut;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new FoodDiaryRepository(_writeDbContext!);
    }

    [Fact]
    public async Task AddAsync_ShouldAddFoodDiary()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );
        await _writeDbContext.Foods.AddAsync(food);
        await _writeDbContext.SaveChangesAsync();

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            date: date,
            foodId: food.Id,
            servingSizeId: servingSize.Id
        );

        try
        {
            // Act
            await _sut!.AddAsync(foodDiary, CancellationToken.None);
            await _writeDbContext.SaveChangesAsync();

            // Assert
            var result = await _writeDbContext.FoodDiaries.FirstOrDefaultAsync(fd =>
                fd.Id == foodDiary.Id
            );

            result.Should().NotBeNull();
            result!.Id.Should().Be(foodDiary.Id);
            result.UserId.Should().Be(userId);
            result.Date.Should().Be(date);
            result.FoodId.Should().Be(food.Id);
            result.ServingSizeId.Should().Be(servingSize.Id);
            result.Quantity.Should().Be(foodDiary.Quantity);
            result.MealType.Should().Be(foodDiary.MealType);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnFoodDiary()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );
        await _writeDbContext.Foods.AddAsync(food);
        await _writeDbContext.SaveChangesAsync();

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            date: date,
            foodId: food.Id,
            servingSizeId: servingSize.Id
        );

        await _writeDbContext.FoodDiaries.AddAsync(foodDiary);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByIdAsync(foodDiary.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(foodDiary.Id);
            result.UserId.Should().Be(userId);
            result.Date.Should().Be(date);
            result.FoodId.Should().Be(food.Id);
            result.ServingSizeId.Should().Be(servingSize.Id);
            result.Quantity.Should().Be(foodDiary.Quantity);
            result.MealType.Should().Be(foodDiary.MealType);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = NutritionDiaryId.NewId();

        try
        {
            // Act
            var result = await _sut!.GetByIdAsync(nonExistentId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateFoodDiary()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );
        await _writeDbContext.Foods.AddAsync(food);
        await _writeDbContext.SaveChangesAsync();

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            date: date,
            foodId: food.Id,
            servingSizeId: servingSize.Id
        );

        await _writeDbContext.FoodDiaries.AddAsync(foodDiary);
        await _writeDbContext.SaveChangesAsync();

        // Update quantity
        foodDiary.UpdateQuantity(2.5f);

        try
        {
            // Act
            _sut!.Update(foodDiary);
            await _writeDbContext.SaveChangesAsync();

            // Assert
            var result = await _writeDbContext.FoodDiaries.FirstOrDefaultAsync(fd =>
                fd.Id == foodDiary.Id
            );

            result.Should().NotBeNull();
            result!.Quantity.Should().Be(2.5f);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteFoodDiary()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food = FoodFaker.Generate(
            foodServingSizes: new List<FoodServingSize>
            {
                FoodServingSizeFaker.Generate(0, servingSize: servingSize),
            }
        );
        await _writeDbContext.Foods.AddAsync(food);
        await _writeDbContext.SaveChangesAsync();

        var foodDiary = FoodDiaryFaker.Generate(
            userId: userId,
            date: date,
            foodId: food.Id,
            servingSizeId: servingSize.Id
        );

        await _writeDbContext.FoodDiaries.AddAsync(foodDiary);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            _sut!.Remove(foodDiary);
            await _writeDbContext.SaveChangesAsync();

            // Assert
            var result = await _writeDbContext.FoodDiaries.FirstOrDefaultAsync(fd =>
                fd.Id == foodDiary.Id
            );

            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
