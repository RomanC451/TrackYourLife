using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodDiaries;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.FoodDiaries;

[Collection("NutritionRepositoryTests")]
public class FoodDiaryQueryTests : BaseRepositoryTests
{
    private FoodDiaryQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new FoodDiaryQuery(_readDbContext!);
    }

    [Fact]
    public async Task GetByDateAsync_WhenEntriesExist_ShouldReturnReadModels()
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
            var result = await _sut!.GetByDateAsync(userId, date, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            var entry = result.First();
            entry.Id.Should().Be(foodDiary.Id);
            entry.UserId.Should().Be(userId);
            entry.Date.Should().Be(date);
            entry.Food.Should().NotBeNull();
            entry.Food!.Name.Should().Be(food.Name);
            entry.ServingSize.Should().NotBeNull();
            entry.ServingSize!.Unit.Should().Be(servingSize.Unit);
            entry.ServingSize!.Value.Should().Be(servingSize.Value);
            entry.Quantity.Should().Be(foodDiary.Quantity);
            entry.MealType.Should().Be(foodDiary.MealType);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByDateAsync_WhenNoEntriesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        try
        {
            // Act
            var result = await _sut!.GetByDateAsync(userId, date, CancellationToken.None);

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
    public async Task GetByPeriodAsync_WhenEntriesExist_ShouldReturnReadModels()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        // Create and save ServingSize first
        var servingSize = ServingSizeFaker.Generate();
        await _writeDbContext!.ServingSizes.AddAsync(servingSize);
        await _writeDbContext.SaveChangesAsync();

        // Create and save Food with FoodServingSize
        var food = FoodFaker.Generate(
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );
        await _writeDbContext.Foods.AddAsync(food);
        await _writeDbContext.SaveChangesAsync();

        var foodDiaries = new List<FoodDiary>
        {
            FoodDiaryFaker.Generate(
                userId: userId,
                date: startDate,
                foodId: food.Id,
                servingSizeId: servingSize.Id
            ),
            FoodDiaryFaker.Generate(
                userId: userId,
                date: startDate.AddDays(1),
                foodId: food.Id,
                servingSizeId: servingSize.Id
            ),
            FoodDiaryFaker.Generate(
                userId: userId,
                date: startDate.AddDays(2),
                foodId: food.Id,
                servingSizeId: servingSize.Id
            ),
        };

        await _writeDbContext.FoodDiaries.AddRangeAsync(foodDiaries);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(3);
            resultList.Should().BeInAscendingOrder(x => x.CreatedOnUtc);

            // Check key properties of each item by matching by ID
            foreach (var actual in resultList)
            {
                var expected = foodDiaries.First(fd => fd.Id == actual.Id);

                actual.Id.Should().Be(expected.Id);
                actual.UserId.Should().Be(expected.UserId);
                actual.Date.Should().Be(expected.Date);
                actual.Quantity.Should().Be(expected.Quantity);
                actual.MealType.Should().Be(expected.MealType);

                // Check Food properties
                actual.Food.Should().NotBeNull();
                actual.Food!.Id.Should().Be(food.Id);
                actual.Food.Name.Should().Be(food.Name);
                actual.Food.BrandName.Should().Be(food.BrandName);
                actual.Food.CountryCode.Should().Be(food.CountryCode);

                // Check ServingSize properties
                actual.ServingSize.Should().NotBeNull();
                actual.ServingSize!.Id.Should().Be(servingSize.Id);
                actual.ServingSize.Unit.Should().Be(servingSize.Unit);
                actual.ServingSize.Value.Should().Be(servingSize.Value);
                actual.ServingSize.NutritionMultiplier.Should().Be(servingSize.NutritionMultiplier);
            }
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByPeriodAsync_WhenNoEntriesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

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
    public async Task GetByPeriodAsync_ShouldOnlyReturnEntriesForSpecifiedUser()
    {
        // Arrange
        var userId1 = UserId.NewId();
        var userId2 = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

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

        var foodDiariesUser1 = new List<FoodDiary>
        {
            FoodDiaryFaker.Generate(
                userId: userId1,
                date: startDate,
                foodId: food.Id,
                servingSizeId: servingSize.Id
            ),
            FoodDiaryFaker.Generate(
                userId: userId1,
                date: startDate.AddDays(1),
                foodId: food.Id,
                servingSizeId: servingSize.Id
            ),
        };

        var foodDiariesUser2 = new List<FoodDiary>
        {
            FoodDiaryFaker.Generate(
                userId: userId2,
                date: startDate,
                foodId: food.Id,
                servingSizeId: servingSize.Id
            ),
            FoodDiaryFaker.Generate(
                userId: userId2,
                date: startDate.AddDays(1),
                foodId: food.Id,
                servingSizeId: servingSize.Id
            ),
        };

        await _writeDbContext.FoodDiaries.AddRangeAsync(foodDiariesUser1);
        await _writeDbContext.FoodDiaries.AddRangeAsync(foodDiariesUser2);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId1,
                startDate,
                endDate,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(2);
            resultList.Should().BeInAscendingOrder(x => x.CreatedOnUtc);

            // Check key properties of each item
            for (int i = 0; i < resultList.Count; i++)
            {
                var actual = resultList[i];
                var expected = foodDiariesUser1[i];

                actual.Id.Should().Be(expected.Id);
                actual.UserId.Should().Be(expected.UserId);
                actual.Date.Should().Be(expected.Date);
                actual.Quantity.Should().Be(expected.Quantity);
                actual.MealType.Should().Be(expected.MealType);

                // Check Food properties
                actual.Food.Should().NotBeNull();
                actual.Food!.Id.Should().Be(food.Id);
                actual.Food.Name.Should().Be(food.Name);
                actual.Food.BrandName.Should().Be(food.BrandName);
                actual.Food.CountryCode.Should().Be(food.CountryCode);

                // Check ServingSize properties
                actual.ServingSize.Should().NotBeNull();
                actual.ServingSize!.Id.Should().Be(servingSize.Id);
                actual.ServingSize.Unit.Should().Be(servingSize.Unit);
                actual.ServingSize.Value.Should().Be(servingSize.Value);
                actual.ServingSize.NutritionMultiplier.Should().Be(servingSize.NutritionMultiplier);
            }
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByPeriodAsync_WithEmptyDateRange_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate; // Same day

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

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
    public async Task GetByPeriodAsync_WithInvalidDateRange_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(-1); // End date before start date

        try
        {
            // Act
            var result = await _sut!.GetByPeriodAsync(
                userId,
                startDate,
                endDate,
                CancellationToken.None
            );

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
