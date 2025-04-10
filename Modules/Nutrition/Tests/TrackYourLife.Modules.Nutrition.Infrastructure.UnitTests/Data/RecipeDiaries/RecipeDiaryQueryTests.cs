using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.RecipeDiaries;

[Collection("NutritionRepositoryTests")]
public class RecipeDiaryQueryTests : BaseRepositoryTests
{
    private RecipeDiaryQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new RecipeDiaryQuery(_readDbContext!);
    }

    [Fact]
    public async Task AnyByRecipeIdAsync_WhenEntriesExist_ShouldReturnTrue()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipe = Recipe.Create(RecipeId.NewId(), userId, "Test Recipe").Value;
        await _writeDbContext!.Recipes.AddAsync(recipe);
        await _writeDbContext.SaveChangesAsync();

        var recipeDiary = RecipeDiaryFaker.Generate(
            userId: userId,
            recipeId: recipe.Id,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        await _writeDbContext.RecipeDiaries.AddAsync(recipeDiary);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.AnyByRecipeIdAsync(userId, recipe.Id, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task AnyByRecipeIdAsync_WhenNoEntriesExist_ShouldReturnFalse()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();

        try
        {
            // Act
            var result = await _sut.AnyByRecipeIdAsync(userId, recipeId, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByDateAsync_WhenEntriesExist_ShouldReturnReadModels()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Create and save Recipe first
        var recipe = Recipe.Create(RecipeId.NewId(), userId, "Test Recipe").Value;
        await _writeDbContext!.Recipes.AddAsync(recipe);
        await _writeDbContext.SaveChangesAsync();

        var recipeDiary = RecipeDiaryFaker.Generate(
            userId: userId,
            recipeId: recipe.Id,
            date: date
        );

        await _writeDbContext.RecipeDiaries.AddAsync(recipeDiary);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByDateAsync(userId, date, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(1);
            var entry = resultList[0];
            entry.Id.Should().Be(recipeDiary.Id);
            entry.UserId.Should().Be(userId);
            entry.Date.Should().Be(date);
            entry.Recipe.Should().NotBeNull();
            entry.Recipe.Id.Should().Be(recipe.Id);
            entry.Recipe.Name.Should().Be(recipe.Name);
            entry.Quantity.Should().Be(recipeDiary.Quantity);
            entry.MealType.Should().Be(recipeDiary.MealType);
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
            var result = await _sut.GetByDateAsync(userId, date, CancellationToken.None);

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

        // Create and save Recipe first
        var recipe = Recipe.Create(RecipeId.NewId(), userId, "Test Recipe").Value;
        await _writeDbContext!.Recipes.AddAsync(recipe);
        await _writeDbContext.SaveChangesAsync();

        var recipeDiaries = new List<RecipeDiary>
        {
            RecipeDiaryFaker.Generate(userId: userId, recipeId: recipe.Id, date: startDate),
            RecipeDiaryFaker.Generate(
                userId: userId,
                recipeId: recipe.Id,
                date: startDate.AddDays(1)
            ),
            RecipeDiaryFaker.Generate(
                userId: userId,
                recipeId: recipe.Id,
                date: startDate.AddDays(2)
            ),
        };

        await _writeDbContext.RecipeDiaries.AddRangeAsync(recipeDiaries);
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

            // Check key properties of each item
            for (int i = 0; i < resultList.Count; i++)
            {
                var actual = resultList[i];
                var expected = recipeDiaries[i];

                actual.Id.Should().Be(expected.Id);
                actual.UserId.Should().Be(expected.UserId);
                actual.Date.Should().Be(expected.Date);
                actual.Quantity.Should().Be(expected.Quantity);
                actual.MealType.Should().Be(expected.MealType);

                // Check Recipe properties
                actual.Recipe.Should().NotBeNull();
                actual.Recipe.Id.Should().Be(recipe.Id);
                actual.Recipe.Name.Should().Be(recipe.Name);
                actual.Recipe.Portions.Should().Be(recipe.Portions);
                actual.Recipe.IsOld.Should().Be(recipe.IsOld);
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
            var result = await _sut.GetByPeriodAsync(
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

        // Create and save Recipe first
        var recipe = Recipe.Create(RecipeId.NewId(), userId1, "Test Recipe").Value;
        await _writeDbContext!.Recipes.AddAsync(recipe);
        await _writeDbContext.SaveChangesAsync();

        var recipeDiariesUser1 = new List<RecipeDiary>
        {
            RecipeDiaryFaker.Generate(userId: userId1, recipeId: recipe.Id, date: startDate),
            RecipeDiaryFaker.Generate(
                userId: userId1,
                recipeId: recipe.Id,
                date: startDate.AddDays(1)
            ),
        };

        var recipeDiariesUser2 = new List<RecipeDiary>
        {
            RecipeDiaryFaker.Generate(userId: userId2, recipeId: recipe.Id, date: startDate),
            RecipeDiaryFaker.Generate(
                userId: userId2,
                recipeId: recipe.Id,
                date: startDate.AddDays(1)
            ),
        };

        await _writeDbContext.RecipeDiaries.AddRangeAsync(recipeDiariesUser1);
        await _writeDbContext.RecipeDiaries.AddRangeAsync(recipeDiariesUser2);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByPeriodAsync(
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
                var expected = recipeDiariesUser1[i];

                actual.Id.Should().Be(expected.Id);
                actual.UserId.Should().Be(expected.UserId);
                actual.Date.Should().Be(expected.Date);
                actual.Quantity.Should().Be(expected.Quantity);
                actual.MealType.Should().Be(expected.MealType);

                // Check Recipe properties
                actual.Recipe.Should().NotBeNull();
                actual.Recipe.Id.Should().Be(recipe.Id);
                actual.Recipe.Name.Should().Be(recipe.Name);
                actual.Recipe.Portions.Should().Be(recipe.Portions);
                actual.Recipe.IsOld.Should().Be(recipe.IsOld);
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
            var result = await _sut.GetByPeriodAsync(
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
            var result = await _sut.GetByPeriodAsync(
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
