using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.FunctionalTests.Utils;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;
using TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests.Features;

[Collection("Nutrition Integration Tests")]
public class RecipesTests(NutritionFunctionalTestWebAppFactory factory)
    : NutritionBaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateRecipe_WithValidData_ShouldCreateRecipe()
    {
        // Arrange
        var request = new CreateRecipeRequest("Test Recipe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/recipes", request);

        // Assert
        var responseContent = await response.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        // Verify database state
        var recipe = await _nutritionWriteDbContext.Recipes.FirstOrDefaultAsync(r =>
            r.Id == new RecipeId(responseContent.id)
        );

        recipe.Should().NotBeNull();
        recipe!.Name.Should().Be(request.Name);
        recipe.UserId.Should().Be(_user.Id);
        recipe.Portions.Should().Be(1); // Default value
        recipe.Ingredients.Should().BeEmpty();
        recipe.NutritionalContents.Should().NotBeNull();
        recipe.IsOld.Should().BeFalse();
        recipe.CreatedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        recipe.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public async Task UpdateRecipe_WithValidData_ShouldUpdateRecipe()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Create recipe diary entries
        var diaryRequest = new AddRecipeDiaryRequest(
            RecipeId: new RecipeId(recipeId),
            MealType: MealTypes.Breakfast,
            Quantity: 100f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow)
        );
        await (
            await _client.PostAsJsonAsync("/api/recipe-diaries", diaryRequest)
        ).ShouldHaveStatusCode(HttpStatusCode.Created);

        // Create another diary entry for a different meal
        var lunchDiaryRequest = new AddRecipeDiaryRequest(
            RecipeId: new RecipeId(recipeId),
            MealType: MealTypes.Lunch,
            Quantity: 150f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow)
        );
        await (
            await _client.PostAsJsonAsync("/api/recipe-diaries", lunchDiaryRequest)
        ).ShouldHaveStatusCode(HttpStatusCode.Created);

        // Update the recipe
        var updateRequest = new UpdateRecipeRequest("Updated Recipe", 2);
        await (
            await _client.PutAsJsonAsync($"/api/recipes/{recipeId}", updateRequest)
        ).ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Verify recipe was updated in place
        var recipe = await _nutritionWriteDbContext.Recipes.FirstOrDefaultAsync(r =>
            r.Id == new RecipeId(recipeId)
        );
        recipe.Should().NotBeNull();
        recipe!.Name.Should().Be(updateRequest.Name);
        recipe.Portions.Should().Be(updateRequest.Portions);
        recipe.IsOld.Should().BeFalse();

        // Verify recipe diaries still use the same recipe
        var recipeDiaries = await _nutritionWriteDbContext
            .RecipeDiaries.Where(rd => rd.UserId == _user.Id)
            .ToListAsync();

        recipeDiaries.Should().HaveCount(2);
        recipeDiaries.Should().AllSatisfy(rd => rd.RecipeId.Should().Be(new RecipeId(recipeId)));

        // Verify no new recipe was created
        var recipeCount = await _nutritionWriteDbContext.Recipes.CountAsync(r =>
            r.UserId == _user.Id
        );
        recipeCount.Should().Be(1);
    }

    [Fact]
    public async Task UpdateRecipe_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Create another recipe with different name
        var secondCreateRequest = new CreateRecipeRequest("Second Recipe");
        await (
            await _client.PostAsJsonAsync("/api/recipes", secondCreateRequest)
        ).ShouldHaveStatusCode(HttpStatusCode.Created);

        // Try to update first recipe with second recipe's name
        var updateRequest = new UpdateRecipeRequest("Second Recipe", 2);
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/recipes/{recipeId}",
            updateRequest
        );

        // Assert
        await updateResponse.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
        var problemDetails = await updateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Bad Request");
    }

    [Fact]
    public async Task UpdateRecipe_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var updateRequest = new UpdateRecipeRequest("Updated Recipe", 2);

        // Act
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/recipes/{nonExistingId}",
            updateRequest
        );

        // Assert
        await updateResponse.ShouldHaveStatusCode(HttpStatusCode.NotFound);
        var problemDetails = await updateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Not Found");
    }

    [Fact]
    public async Task UpdateRecipe_WithInvalidPortions_ShouldReturnBadRequest()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Try to update recipe with invalid portions
        var updateRequest = new UpdateRecipeRequest("Updated Recipe", 0);
        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/recipes/{recipeId}",
            updateRequest
        );

        // Assert
        await updateResponse.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
        var problemDetails = await updateResponse.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Validation Error");
    }

    [Fact]
    public async Task DeleteRecipe_WithValidId_ShouldDeleteRecipe()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/recipes/{recipeId}");

        // Assert
        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);

        // Verify recipe is marked as deleted
        var recipe = await _nutritionWriteDbContext.Recipes.FirstOrDefaultAsync(r =>
            r.Id == new RecipeId(recipeId)
        );
        recipe.Should().NotBeNull();
        recipe!.IsOld.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteRecipe_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/recipes/{nonExistingId}");

        // Assert
        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UndoDeleteRecipe_WithValidId_ShouldRestoreRecipe()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Delete the recipe
        await _client.DeleteAsync($"/api/recipes/{recipeId}");

        // Act
        var undoResponse = await _client.PostAsJsonAsync(
            "/api/recipes/undo-delete",
            new UndoDeleteRecipeRequest(new RecipeId(recipeId))
        );

        // Assert
        undoResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);

        // Verify recipe is restored
        var recipe = await _nutritionWriteDbContext.Recipes.FirstOrDefaultAsync(r =>
            r.Id == new RecipeId(recipeId)
        );
        recipe.Should().NotBeNull();
        recipe!.IsOld.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteRecipes_WithValidIds_ShouldDeleteMultipleRecipes()
    {
        // Arrange
        var recipeIds = new List<RecipeId>();
        for (int i = 0; i < 3; i++)
        {
            var createRequest = new CreateRecipeRequest($"Test Recipe {i}");
            var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
            recipeIds.Add(
                new RecipeId(
                    (
                        await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                            HttpStatusCode.Created
                        )
                    ).id
                )
            );
        }

        // Act
        var deleteResponse = await _client.DeleteAsJsonAsync(
            "/api/recipes/batch",
            new DeleteRecipesRequest(recipeIds)
        );

        // Assert
        deleteResponse.Should().HaveStatusCode(HttpStatusCode.NoContent);

        // Verify all recipes are marked as deleted
        var recipes = await _nutritionWriteDbContext
            .Recipes.Where(r => recipeIds.Contains(r.Id))
            .ToListAsync();
        recipes.Should().HaveCount(3);
        recipes.Should().AllSatisfy(r => r.IsOld.Should().BeTrue());
    }

    [Fact]
    public async Task AddIngredient_WhenRecipeUsedInDiary_ShouldCloneRecipe()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Create recipe diary entries
        var diaryRequest = new AddRecipeDiaryRequest(
            RecipeId: new RecipeId(recipeId),
            MealType: MealTypes.Breakfast,
            Quantity: 100f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow)
        );
        await (
            await _client.PostAsJsonAsync("/api/recipe-diaries", diaryRequest)
        ).ShouldHaveStatusCode(HttpStatusCode.Created);

        // Create a serving size first
        var servingSizeId = new ServingSizeId(Guid.NewGuid());
        var servingSizeResult = ServingSize.Create(servingSizeId, 1f, "g", 100f, null);
        servingSizeResult.IsSuccess.Should().BeTrue();
        var servingSize = servingSizeResult.Value;
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Create a food with a serving size
        var foodId = new FoodId(Guid.NewGuid());
        var foodServingSizeResult = FoodServingSize.Create(foodId, servingSizeId, 0);
        foodServingSizeResult.IsSuccess.Should().BeTrue();
        var foodServingSize = foodServingSizeResult.Value;
        var foodResult = Food.Create(
            foodId,
            "Test",
            "Test Brand",
            "US",
            "Test Food",
            new NutritionalContent(),
            new List<FoodServingSize> { foodServingSize },
            null
        );
        foodResult.IsSuccess.Should().BeTrue();
        var food = foodResult.Value;
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var addIngredientRequest = new AddIngredientRequest(
            FoodId: foodId,
            Quantity: 100f,
            ServingSizeId: servingSizeId
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/recipes/{recipeId}/ingredients",
            addIngredientRequest
        );

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);

        // Verify original recipe is not marked as old
        var originalRecipe = await _nutritionWriteDbContext
            .Recipes.Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == new RecipeId(recipeId));
        originalRecipe.Should().NotBeNull();
        originalRecipe!.IsOld.Should().BeFalse();
        originalRecipe.Ingredients.Should().HaveCount(1);
        originalRecipe.Ingredients[0].Quantity.Should().Be(addIngredientRequest.Quantity);

        // Verify a new recipe was created and marked as old
        var recipes = await _nutritionWriteDbContext
            .Recipes.Include(r => r.Ingredients)
            .Where(r => r.UserId == _user.Id)
            .ToListAsync();
        recipes.Should().HaveCount(2);

        // Verify the cloned recipe is marked as old and has no ingredients
        var clonedRecipe = recipes.First(r => r.IsOld);
        clonedRecipe.Name.Should().Be(createRequest.Name);
        clonedRecipe.Portions.Should().Be(1); // Default value
        clonedRecipe.Ingredients.Should().BeEmpty();

        // Verify recipe diaries use the original recipe
        var recipeDiaries = await _nutritionWriteDbContext
            .RecipeDiaries.Where(rd => rd.UserId == _user.Id)
            .ToListAsync();
        recipeDiaries.Should().HaveCount(1);
        recipeDiaries[0].RecipeId.Should().Be(clonedRecipe.Id);
    }

    [Fact]
    public async Task AddIngredient_WhenRecipeNotUsedInDiary_ShouldNotCloneRecipe()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Create a serving size first
        var servingSizeId = new ServingSizeId(Guid.NewGuid());
        var servingSizeResult = ServingSize.Create(servingSizeId, 1f, "g", 100f, null);
        servingSizeResult.IsSuccess.Should().BeTrue();
        var servingSize = servingSizeResult.Value;
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Create a food with a serving size
        var foodId = new FoodId(Guid.NewGuid());
        var foodServingSizeResult = FoodServingSize.Create(foodId, servingSizeId, 0);
        foodServingSizeResult.IsSuccess.Should().BeTrue();
        var foodServingSize = foodServingSizeResult.Value;
        var foodResult = Food.Create(
            foodId,
            "Test",
            "Test Brand",
            "US",
            "Test Food",
            new NutritionalContent(),
            new List<FoodServingSize> { foodServingSize },
            null
        );
        foodResult.IsSuccess.Should().BeTrue();
        var food = foodResult.Value;
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var addIngredientRequest = new AddIngredientRequest(
            FoodId: foodId,
            Quantity: 100f,
            ServingSizeId: servingSizeId
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/recipes/{recipeId}/ingredients",
            addIngredientRequest
        );

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);

        // Verify recipe was updated in place
        var recipe = await _nutritionWriteDbContext
            .Recipes.Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == new RecipeId(recipeId));
        recipe.Should().NotBeNull();
        recipe!.IsOld.Should().BeFalse();
        recipe.Ingredients.Should().HaveCount(1);
        recipe.Ingredients[0].Quantity.Should().Be(addIngredientRequest.Quantity);

        // Verify no new recipe was created
        var recipeCount = await _nutritionWriteDbContext.Recipes.CountAsync(r =>
            r.UserId == _user.Id
        );
        recipeCount.Should().Be(1);
    }

    [Fact]
    public async Task AddIngredient_WhenRecipeUsedInMultipleDiaries_ShouldCloneRecipeOnce()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Create multiple recipe diary entries
        var diaryRequest = new AddRecipeDiaryRequest(
            RecipeId: new RecipeId(recipeId),
            MealType: MealTypes.Breakfast,
            Quantity: 100f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow)
        );
        await (
            await _client.PostAsJsonAsync("/api/recipe-diaries", diaryRequest)
        ).ShouldHaveStatusCode(HttpStatusCode.Created);

        var lunchDiaryRequest = new AddRecipeDiaryRequest(
            RecipeId: new RecipeId(recipeId),
            MealType: MealTypes.Lunch,
            Quantity: 150f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow)
        );
        await (
            await _client.PostAsJsonAsync("/api/recipe-diaries", lunchDiaryRequest)
        ).ShouldHaveStatusCode(HttpStatusCode.Created);

        // Create a serving size first
        var servingSizeId = new ServingSizeId(Guid.NewGuid());
        var servingSizeResult = ServingSize.Create(servingSizeId, 1f, "g", 100f, null);
        servingSizeResult.IsSuccess.Should().BeTrue();
        var servingSize = servingSizeResult.Value;
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Create a food with a serving size
        var foodId = new FoodId(Guid.NewGuid());
        var foodServingSizeResult = FoodServingSize.Create(foodId, servingSizeId, 0);
        foodServingSizeResult.IsSuccess.Should().BeTrue();
        var foodServingSize = foodServingSizeResult.Value;
        var foodResult = Food.Create(
            foodId,
            "Test",
            "Test Brand",
            "US",
            "Test Food",
            new NutritionalContent(),
            new List<FoodServingSize> { foodServingSize },
            null
        );
        foodResult.IsSuccess.Should().BeTrue();
        var food = foodResult.Value;
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var addIngredientRequest = new AddIngredientRequest(
            FoodId: foodId,
            Quantity: 100f,
            ServingSizeId: servingSizeId
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/recipes/{recipeId}/ingredients",
            addIngredientRequest
        );

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);

        // Verify only one new recipe was created and marked as old
        var recipes = await _nutritionWriteDbContext
            .Recipes.Include(r => r.Ingredients)
            .Where(r => r.UserId == _user.Id)
            .ToListAsync();
        recipes.Should().HaveCount(2);

        // Verify the cloned recipe is marked as old and has no ingredients
        var clonedRecipe = recipes.First(r => r.IsOld);
        clonedRecipe.Name.Should().Be(createRequest.Name);
        clonedRecipe.Portions.Should().Be(1); // Default value
        clonedRecipe.Ingredients.Should().BeEmpty();

        // Verify original recipe is not marked as old and has the new ingredient
        var originalRecipe = await _nutritionWriteDbContext
            .Recipes.Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == new RecipeId(recipeId));
        originalRecipe.Should().NotBeNull();
        originalRecipe!.IsOld.Should().BeFalse();
        originalRecipe.Ingredients.Should().HaveCount(1);
        originalRecipe.Ingredients[0].Quantity.Should().Be(addIngredientRequest.Quantity);

        // Verify all recipe diaries use the original recipe
        var recipeDiaries = await _nutritionWriteDbContext
            .RecipeDiaries.Where(rd => rd.UserId == _user.Id)
            .ToListAsync();
        recipeDiaries.Should().HaveCount(2);
        recipeDiaries.Should().AllSatisfy(rd => rd.RecipeId.Should().Be(clonedRecipe.Id));
    }

    [Fact]
    public async Task UpdateIngredient_WithValidData_ShouldUpdateIngredient()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Create a serving size first
        var servingSizeId = new ServingSizeId(Guid.NewGuid());
        var servingSizeResult = ServingSize.Create(servingSizeId, 1f, "g", 100f, null);
        servingSizeResult.IsSuccess.Should().BeTrue();
        var servingSize = servingSizeResult.Value;
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Create a food with a serving size
        var foodId = new FoodId(Guid.NewGuid());
        var foodServingSizeResult = FoodServingSize.Create(foodId, servingSizeId, 0);
        foodServingSizeResult.IsSuccess.Should().BeTrue();
        var foodServingSize = foodServingSizeResult.Value;
        var foodResult = Food.Create(
            foodId,
            "Test",
            "Test Brand",
            "US",
            "Test Food",
            new NutritionalContent(),
            new List<FoodServingSize> { foodServingSize },
            null
        );
        foodResult.IsSuccess.Should().BeTrue();
        var food = foodResult.Value;
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Add an ingredient first
        var addIngredientRequest = new AddIngredientRequest(
            FoodId: foodId,
            Quantity: 100f,
            ServingSizeId: servingSizeId
        );
        var addResponse = await _client.PostAsJsonAsync(
            $"/api/recipes/{recipeId}/ingredients",
            addIngredientRequest
        );
        var ingredientId = (
            await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(HttpStatusCode.Created)
        ).id;

        var updateIngredientRequest = new UpdateIngredientRequest(
            ServingSizeId: servingSizeId,
            Quantity: 200f
        );

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/api/recipes/{recipeId}/ingredients/{ingredientId}",
            updateIngredientRequest
        );

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        // Verify ingredient was updated
        var recipe = await _nutritionWriteDbContext
            .Recipes.Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == new RecipeId(recipeId));
        recipe.Should().NotBeNull();
        recipe!.Ingredients.Should().HaveCount(1);
        recipe.Ingredients[0].Quantity.Should().Be(updateIngredientRequest.Quantity);
    }

    [Fact]
    public async Task RemoveIngredient_WithValidId_ShouldRemoveIngredient()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Create a serving size first
        var servingSizeId = new ServingSizeId(Guid.NewGuid());
        var servingSizeResult = ServingSize.Create(servingSizeId, 1f, "g", 100f, null);
        servingSizeResult.IsSuccess.Should().BeTrue();
        var servingSize = servingSizeResult.Value;
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Create a food with a serving size
        var foodId = new FoodId(Guid.NewGuid());
        var foodServingSizeResult = FoodServingSize.Create(foodId, servingSizeId, 0);
        foodServingSizeResult.IsSuccess.Should().BeTrue();
        var foodServingSize = foodServingSizeResult.Value;
        var foodResult = Food.Create(
            foodId,
            "Test",
            "Test Brand",
            "US",
            "Test Food",
            new NutritionalContent(),
            new List<FoodServingSize> { foodServingSize },
            null
        );
        foodResult.IsSuccess.Should().BeTrue();
        var food = foodResult.Value;
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Add an ingredient first
        var addIngredientRequest = new AddIngredientRequest(
            FoodId: foodId,
            Quantity: 100f,
            ServingSizeId: servingSizeId
        );
        var addResponse = await _client.PostAsJsonAsync(
            $"/api/recipes/{recipeId}/ingredients",
            addIngredientRequest
        );
        var ingredientId = (
            await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(HttpStatusCode.Created)
        ).id;

        // Act
        var removeRequest = new RemoveIngredientsRequest(
            new List<IngredientId> { new(ingredientId) }
        );
        var response = await _client.DeleteAsJsonAsync(
            $"/api/recipes/{recipeId}/ingredients",
            removeRequest
        );

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NoContent);

        // Verify ingredient was removed
        var recipe = await _nutritionWriteDbContext
            .Recipes.Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.Id == new RecipeId(recipeId));
        recipe.Should().NotBeNull();
        recipe!.Ingredients.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateRecipe_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateRecipeRequest("");

        // Act
        var response = await _client.PostAsJsonAsync("/api/recipes", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateRecipe_WithDuplicateName_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateRecipeRequest("Test Recipe");
        await _client.PostAsJsonAsync("/api/recipes", request);

        // Act
        var response = await _client.PostAsJsonAsync("/api/recipes", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRecipe_WithValidId_ShouldReturnRecipe()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Act
        var response = await _client.GetAsync($"/api/recipes/{recipeId}");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var recipe = await response.Content.ReadFromJsonAsync<RecipeDto>();
        recipe.Should().NotBeNull();
        recipe!.Name.Should().Be(createRequest.Name);
    }

    [Fact]
    public async Task GetRecipe_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/recipes/{nonExistingId}");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateRecipe_WithEmptyName_ShouldReturnBadRequest()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        var updateRequest = new UpdateRecipeRequest("", 2);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/recipes/{recipeId}", updateRequest);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateRecipe_WhenRecipeIsDeleted_ShouldReturnNotFound()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Delete the recipe
        await _client.DeleteAsync($"/api/recipes/{recipeId}");

        var updateRequest = new UpdateRecipeRequest("Updated Recipe", 2);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/recipes/{recipeId}", updateRequest);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRecipe_WhenRecipeIsAlreadyDeleted_ShouldReturnNotFound()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Delete the recipe first time
        await _client.DeleteAsync($"/api/recipes/{recipeId}");

        // Act
        var response = await _client.DeleteAsync($"/api/recipes/{recipeId}");

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteRecipes_WithEmptyList_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new DeleteRecipesRequest([]);

        // Act
        var response = await _client.DeleteAsJsonAsync("/api/recipes/batch", request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteRecipes_WithNonExistingIds_ShouldReturnNotFound()
    {
        // Arrange
        var request = new DeleteRecipesRequest(new List<RecipeId> { new(Guid.NewGuid()) });

        // Act
        var response = await _client.DeleteAsJsonAsync("/api/recipes/batch", request);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UndoDeleteRecipe_WhenRecipeIsNotDeleted_ShouldReturnNotFound()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/recipes/undo-delete",
            new UndoDeleteRecipeRequest(new RecipeId(recipeId))
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddIngredient_ShouldAddFoodToHistory()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe");
        var createResponse = await _client.PostAsJsonAsync("/api/recipes", createRequest);
        var recipeId = (
            await createResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
                HttpStatusCode.Created
            )
        ).id;

        // Create a serving size first
        var servingSizeId = new ServingSizeId(Guid.NewGuid());
        var servingSizeResult = ServingSize.Create(servingSizeId, 1f, "g", 100f, null);
        servingSizeResult.IsSuccess.Should().BeTrue();
        var servingSize = servingSizeResult.Value;
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Create a food with a serving size
        var foodId = new FoodId(Guid.NewGuid());
        var foodServingSizeResult = FoodServingSize.Create(foodId, servingSizeId, 0);
        foodServingSizeResult.IsSuccess.Should().BeTrue();
        var foodServingSize = foodServingSizeResult.Value;
        var foodResult = Food.Create(
            foodId,
            "Test",
            "Test Brand",
            "US",
            "Test Food",
            new NutritionalContent(),
            new List<FoodServingSize> { foodServingSize },
            null
        );
        foodResult.IsSuccess.Should().BeTrue();
        var food = foodResult.Value;
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var addIngredientRequest = new AddIngredientRequest(
            FoodId: foodId,
            Quantity: 100f,
            ServingSizeId: servingSizeId
        );

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/recipes/{recipeId}/ingredients",
            addIngredientRequest
        );

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify food was added to history
        var foodHistory = await _nutritionWriteDbContext.FoodHistories.FirstOrDefaultAsync(fh =>
            fh.UserId == _user.Id && fh.FoodId == foodId
        );
        foodHistory.Should().NotBeNull();
        foodHistory!.UserId.Should().Be(_user.Id);
        foodHistory.FoodId.Should().Be(foodId);
    }
}
