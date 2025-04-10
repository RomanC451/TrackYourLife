using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Data.RecipeDiaries;

[Collection("NutritionRepositoryTests")]
public class RecipeDiaryRepositoryTests : BaseRepositoryTests
{
    private RecipeDiaryRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new RecipeDiaryRepository(_writeDbContext!);
    }

    [Fact]
    public async Task BulkUpdateRecipeId_WhenEntriesExist_ShouldUpdateAllMatchingEntries()
    {
        // Arrange
        var userId = UserId.NewId();
        var oldRecipeId = RecipeId.NewId();
        var newRecipeId = RecipeId.NewId();

        // Create and save old recipe
        var oldRecipe = Recipe.Create(oldRecipeId, userId, "Old Recipe").Value;
        await _writeDbContext!.Recipes.AddAsync(oldRecipe);
        await _writeDbContext.SaveChangesAsync();

        // Create and save new recipe
        var newRecipe = Recipe.Create(newRecipeId, userId, "New Recipe").Value;
        await _writeDbContext.Recipes.AddAsync(newRecipe);
        await _writeDbContext.SaveChangesAsync();

        // Create recipe diaries with old recipe
        var recipeDiaries = new List<RecipeDiary>
        {
            RecipeDiaryFaker.Generate(
                userId: userId,
                recipeId: oldRecipeId,
                date: DateOnly.FromDateTime(DateTime.UtcNow)
            ),
            RecipeDiaryFaker.Generate(
                userId: userId,
                recipeId: oldRecipeId,
                date: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
            ),
        };

        await _writeDbContext.RecipeDiaries.AddRangeAsync(recipeDiaries);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Verify initial state
            var initialDiaries = await _writeDbContext
                .RecipeDiaries.Where(d => d.RecipeId == oldRecipeId)
                .ToListAsync();
            initialDiaries.Should().HaveCount(2);

            // Act
            await _sut.BulkUpdateRecipeId(oldRecipeId, newRecipeId, CancellationToken.None);

            // Reload the context to get fresh data
            _writeDbContext.ChangeTracker.Clear();

            // Assert
            var updatedDiaries = await _writeDbContext
                .RecipeDiaries.Where(d => d.RecipeId == newRecipeId)
                .ToListAsync();

            updatedDiaries.Should().HaveCount(2);
            foreach (var diary in updatedDiaries)
            {
                diary.RecipeId.Should().Be(newRecipeId);
            }

            // Verify that no diaries remain with the old recipe ID
            var oldRecipeDiaries = await _writeDbContext
                .RecipeDiaries.Where(d => d.RecipeId == oldRecipeId)
                .ToListAsync();

            oldRecipeDiaries.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task BulkUpdateRecipeId_WhenNoMatchingEntries_ShouldNotUpdateAnything()
    {
        // Arrange
        var userId = UserId.NewId();
        var oldRecipeId = RecipeId.NewId();
        var newRecipeId = RecipeId.NewId();
        var otherRecipeId = RecipeId.NewId();

        // Create and save recipes
        var oldRecipe = Recipe.Create(oldRecipeId, userId, "Old Recipe").Value;
        var newRecipe = Recipe.Create(newRecipeId, userId, "New Recipe").Value;
        var otherRecipe = Recipe.Create(otherRecipeId, userId, "Other Recipe").Value;

        await _writeDbContext!.Recipes.AddRangeAsync(oldRecipe, newRecipe, otherRecipe);
        await _writeDbContext.SaveChangesAsync();

        // Create recipe diaries with other recipe
        var recipeDiaries = new List<RecipeDiary>
        {
            RecipeDiaryFaker.Generate(
                userId: userId,
                recipeId: otherRecipeId,
                date: DateOnly.FromDateTime(DateTime.UtcNow)
            ),
            RecipeDiaryFaker.Generate(
                userId: userId,
                recipeId: otherRecipeId,
                date: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
            ),
        };

        await _writeDbContext.RecipeDiaries.AddRangeAsync(recipeDiaries);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            await _sut.BulkUpdateRecipeId(oldRecipeId, newRecipeId, CancellationToken.None);

            // Reload the context to get fresh data
            _writeDbContext.ChangeTracker.Clear();

            // Assert
            var updatedDiaries = await _writeDbContext
                .RecipeDiaries.Where(d => d.RecipeId == newRecipeId)
                .ToListAsync();

            updatedDiaries.Should().BeEmpty();

            var unchangedDiaries = await _writeDbContext
                .RecipeDiaries.Where(d => d.RecipeId == otherRecipeId)
                .ToListAsync();

            unchangedDiaries.Should().HaveCount(2);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task BulkUpdateRecipeId_WhenRecipeDoesNotExist_ShouldNotUpdateAnything()
    {
        // Arrange
        var userId = UserId.NewId();
        var nonExistentRecipeId = RecipeId.NewId();
        var newRecipeId = RecipeId.NewId();

        // Create and save new recipe
        var newRecipe = Recipe.Create(newRecipeId, userId, "New Recipe").Value;
        await _writeDbContext!.Recipes.AddAsync(newRecipe);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act
            await _sut.BulkUpdateRecipeId(nonExistentRecipeId, newRecipeId, CancellationToken.None);

            // Reload the context to get fresh data
            _writeDbContext.ChangeTracker.Clear();

            // Assert
            var updatedDiaries = await _writeDbContext
                .RecipeDiaries.Where(d => d.RecipeId == newRecipeId)
                .ToListAsync();

            updatedDiaries.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task BulkUpdateRecipeId_WhenNewRecipeDoesNotExist_ShouldThrow()
    {
        // Arrange
        var userId = UserId.NewId();
        var oldRecipeId = RecipeId.NewId();
        var nonExistentRecipeId = RecipeId.NewId();

        // Create and save old recipe
        var oldRecipe = Recipe.Create(oldRecipeId, userId, "Old Recipe").Value;
        await _writeDbContext!.Recipes.AddAsync(oldRecipe);
        await _writeDbContext.SaveChangesAsync();

        // Create recipe diaries with old recipe
        var recipeDiaries = new List<RecipeDiary>
        {
            RecipeDiaryFaker.Generate(
                userId: userId,
                recipeId: oldRecipeId,
                date: DateOnly.FromDateTime(DateTime.UtcNow)
            ),
            RecipeDiaryFaker.Generate(
                userId: userId,
                recipeId: oldRecipeId,
                date: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
            ),
        };

        await _writeDbContext.RecipeDiaries.AddRangeAsync(recipeDiaries);
        await _writeDbContext.SaveChangesAsync();

        try
        {
            // Act & Assert
            var act = () =>
                _sut.BulkUpdateRecipeId(oldRecipeId, nonExistentRecipeId, CancellationToken.None);
            await act.Should()
                .ThrowAsync<Npgsql.PostgresException>()
                .WithMessage("*violates foreign key constraint*");

            // Verify that no changes were made
            _writeDbContext.ChangeTracker.Clear();
            var unchangedDiaries = await _writeDbContext
                .RecipeDiaries.Where(d => d.RecipeId == oldRecipeId)
                .ToListAsync();

            unchangedDiaries.Should().HaveCount(2);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
