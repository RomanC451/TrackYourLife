using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.Recipes;

public class RecipeQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private RecipeQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new RecipeQuery(ReadDbContext);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenRecipesExist_ShouldReturnReadModels()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipes = new List<Recipe>
        {
            Recipe.Create(RecipeId.NewId(), userId, "Recipe 1", 100f, 1).Value,
            Recipe.Create(RecipeId.NewId(), userId, "Recipe 2", 200f, 2).Value,
        };

        await WriteDbContext.Recipes.AddRangeAsync(recipes);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByUserIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(2);

            foreach (var recipe in resultList)
            {
                recipe.UserId.Should().Be(userId);
                recipe.IsOld.Should().BeFalse();
            }
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenNoRecipesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByUserIdAsync(userId, CancellationToken.None);

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
    public async Task GetByUserIdAsync_ShouldOnlyReturnNonOldRecipes()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipes = new List<Recipe>
        {
            Recipe.Create(RecipeId.NewId(), userId, "Recipe 1", 100f, 1).Value,
            Recipe.Create(RecipeId.NewId(), userId, "Recipe 2", 200f, 2).Value,
        };

        recipes[1].MarkAsOld();

        await WriteDbContext.Recipes.AddRangeAsync(recipes);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByUserIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(1);
            resultList[0].Name.Should().Be("Recipe 1");
            resultList[0].IsOld.Should().BeFalse();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByNameAndUserIdAsync_WhenRecipeExists_ShouldReturnReadModel()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipe = Recipe.Create(RecipeId.NewId(), userId, "Test Recipe", 100f, 1).Value;
        await WriteDbContext.Recipes.AddAsync(recipe);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByNameAndUserIdAsync(
                "Test Recipe",
                userId,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Test Recipe");
            result.UserId.Should().Be(userId);
            result.IsOld.Should().BeFalse();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByNameAndUserIdAsync_WhenRecipeDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByNameAndUserIdAsync(
                "Non-existent Recipe",
                userId,
                CancellationToken.None
            );

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByNameAndUserIdAsync_ShouldOnlyReturnNonOldRecipes()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipe = Recipe.Create(RecipeId.NewId(), userId, "Test Recipe", 100f, 1).Value;
        recipe.MarkAsOld();
        await WriteDbContext.Recipes.AddAsync(recipe);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByNameAndUserIdAsync(
                "Test Recipe",
                userId,
                CancellationToken.None
            );

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByNameAndUserIdAsync_ShouldReturnRecipesWithMatchingName()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipes = new List<Recipe>
        {
            Recipe.Create(RecipeId.NewId(), userId, "Test Recipe", 100f, 1).Value,
            Recipe.Create(RecipeId.NewId(), userId, "Test Recipe 2", 200f, 2).Value,
            Recipe.Create(RecipeId.NewId(), userId, "Different Recipe", 300f, 3).Value,
        };

        await WriteDbContext.Recipes.AddRangeAsync(recipes);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByNameAndUserIdAsync(
                "Test Recipe",
                userId,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Contain("Test Recipe");
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
