using System.Net;
using System.Net.Http.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Presentation.Contracts;
using TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries.Queries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;
using TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests.Features;

[Collection("Nutrition Integration Tests")]
public class RecipeDiariesTests(NutritionFunctionalTestWebAppFactory factory)
    : NutritionBaseIntegrationTest(factory)
{
    private async Task<Recipe> ArrangeRecipe()
    {
        var recipeId = RecipeId.NewId();

        var servingSize = ServingSizeFaker.Generate();

        var food = FoodFaker.Generate(
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        var ingredient = IngredientFaker.Generate(foodId: food.Id, servingSizeId: servingSize.Id);

        var recipe = RecipeFaker.Generate(
            id: recipeId,
            foods: [food],
            ingredients: [ingredient],
            servingSizes: [ServingSizeFaker.GenerateReadModel(servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.Ingredients.AddAsync(ingredient);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.Recipes.AddAsync(recipe);

        await _nutritionWriteDbContext.SaveChangesAsync();

        return recipe;
    }

    [Fact]
    public async Task AddRecipeDiary_WithValidData_ShouldCreateEntry()
    {
        // Arrange
        var recipe = await ArrangeRecipe();

        var request = new AddRecipeDiaryRequest(
            recipe.Id,
            MealTypes.Breakfast,
            1.5f,
            DateOnly.FromDateTime(DateTime.UtcNow),
            recipe.ServingSizes[0].Id
        );

        // Act
        var response = await _client.PostAsJsonAsync(ApiRoutes.RecipeDiaries, request);

        // Assert
        var content = await response.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        content.Should().NotBeNull();
        content.id.Should().NotBe(Guid.Empty);

        var recipeDiary = await _nutritionWriteDbContext.RecipeDiaries.FindAsync(
            NutritionDiaryId.Create(content.id)
        );
        recipeDiary.Should().NotBeNull();
        recipeDiary!.RecipeId.Should().Be(recipe.Id);
        recipeDiary.MealType.Should().Be(MealTypes.Breakfast);
        recipeDiary.Quantity.Should().Be(1.5f);
        recipeDiary.Date.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task UpdateRecipeDiary_WithValidData_ShouldUpdateEntry()
    {
        // Arrange
        var recipe = await ArrangeRecipe();

        var addRequest = new AddRecipeDiaryRequest(
            recipe.Id,
            MealTypes.Breakfast,
            1.5f,
            DateOnly.FromDateTime(DateTime.UtcNow),
            recipe.ServingSizes[0].Id
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.RecipeDiaries, addRequest);
        addResponse.EnsureSuccessStatusCode();

        var addResult = await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        var updateRequest = new UpdateRecipeDiaryRequest(
            2.0f,
            MealTypes.Lunch,
            recipe.ServingSizes[0].Id,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var response = await _client.PutAsJsonAsync(
            $"{ApiRoutes.RecipeDiaries}/{addResult.id}",
            updateRequest
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var recipeDiary = await _nutritionWriteDbContext.RecipeDiaries.FindAsync(
            NutritionDiaryId.Create(addResult.id)
        );
        recipeDiary.Should().NotBeNull();
        recipeDiary!.RecipeId.Should().Be(recipe.Id);
        recipeDiary.MealType.Should().Be(MealTypes.Lunch);
        recipeDiary.Quantity.Should().Be(2.0f);
        recipeDiary.Date.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task DeleteRecipeDiary_WithValidId_ShouldDeleteEntry()
    {
        // Arrange
        var recipe = await ArrangeRecipe();

        var addRequest = new AddRecipeDiaryRequest(
            recipe.Id,
            MealTypes.Breakfast,
            1.5f,
            DateOnly.FromDateTime(DateTime.UtcNow),
            recipe.ServingSizes[0].Id
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.RecipeDiaries, addRequest);
        addResponse.EnsureSuccessStatusCode();
        var addResult = await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        // Act
        var response = await _client.DeleteAsync($"{ApiRoutes.RecipeDiaries}/{addResult.id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetRecipeDiaryById_WithValidId_ShouldReturnEntry()
    {
        // Arrange
        var recipe = await ArrangeRecipe();

        var addRequest = new AddRecipeDiaryRequest(
            recipe.Id,
            MealTypes.Breakfast,
            1.5f,
            DateOnly.FromDateTime(DateTime.UtcNow),
            recipe.ServingSizes[0].Id
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.RecipeDiaries, addRequest);
        addResponse.EnsureSuccessStatusCode();
        var addResult = await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        // Act
        var response = await _client.GetAsync($"{ApiRoutes.RecipeDiaries}/{addResult.id}");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<RecipeDiaryDto>(
            HttpStatusCode.OK
        );

        result.Should().NotBeNull();
        result.Id.Value.Should().Be(addResult.id);
        result.Recipe.Id.Should().Be(recipe.Id);
        result.Quantity.Should().Be(1.5f);
        result.MealType.Should().Be(MealTypes.Breakfast);
    }

    [Fact]
    public async Task GetRecipeDiaryById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync(
            $"{ApiRoutes.RecipeDiaries}/{NutritionDiaryId.NewId().Value}"
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetNutritionDiariesByDate_WithValidData_ShouldReturnEntries()
    {
        // Arrange
        var recipe = await ArrangeRecipe();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new AddRecipeDiaryRequest(
            recipe.Id,
            MealTypes.Breakfast,
            1.5f,
            date,
            recipe.ServingSizes[0].Id
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.RecipeDiaries, request);
        addResponse.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"{ApiRoutes.NutritionDiaries}/{date:yyyy-MM-dd}");

        // Assert
        var content =
            await response.ShouldHaveStatusCodeAndContent<GetNutritionDiariesByDateResponse>(
                HttpStatusCode.OK
            );
        content.Diaries.Should().NotBeNullOrEmpty();

        content.Diaries[MealTypes.Lunch].Should().BeEmpty();
        content.Diaries[MealTypes.Dinner].Should().BeEmpty();
        content.Diaries[MealTypes.Snacks].Should().BeEmpty();

        content.Diaries[MealTypes.Breakfast].Should().HaveCount(1);

        var diary = content.Diaries[MealTypes.Breakfast][0];

        diary.Id.Value.Should().NotBe(Guid.Empty);
        diary.Quantity.Should().Be(1.5f);
        diary.MealType.Should().Be(MealTypes.Breakfast);
        diary.Date.Should().Be(date);
    }

    [Fact]
    public async Task GetNutritionOverviewByPeriod_WithValidData_ShouldReturnTotal()
    {
        // Arrange
        var recipe = await ArrangeRecipe();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new AddRecipeDiaryRequest(
            recipe.Id,
            MealTypes.Breakfast,
            1.5f,
            date,
            recipe.ServingSizes[0].Id
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.RecipeDiaries, request);
        addResponse.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync(
            $"{ApiRoutes.NutritionDiaries}/nutrition-overview?startDate={date:yyyy-MM-dd}&endDate={date:yyyy-MM-dd}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<NutritionalContent>();
        result.Should().NotBeNull();
        result!.Energy.Value.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetNutritionOverviewByPeriod_WithNoData_ShouldReturnZero()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var response = await _client.GetAsync(
            $"{ApiRoutes.NutritionDiaries}/nutrition-overview?startDate={date}&endDate={date}"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<NutritionalContent>();
        result.Should().NotBeNull();
        result!.Energy.Value.Should().Be(0);
    }

    [Fact]
    public async Task AddIngredient_ShouldAddFoodToHistory()
    {
        // Arrange
        var createRequest = new CreateRecipeRequest("Test Recipe", 1, 100f);
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

    [Fact]
    public async Task AddRecipeDiary_ShouldUpdateDailyNutritionOverview()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var servingSizeReadModel = ServingSizeFaker.GenerateReadModel(
            id: servingSize.Id,
            unit: servingSize.Unit,
            value: servingSize.Value,
            apiId: servingSize.ApiId
        );

        var food = FoodFaker.Generate(
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );
        var ingredient = IngredientFaker.Generate(foodId: food.Id, servingSizeId: servingSize.Id);
        var recipe = RecipeFaker.Generate(
            ingredients: [ingredient],
            foods: [food],
            servingSizes: [servingSizeReadModel],
            portions: 6
        );

        var testHarness = GetTestHarness();

        // Register handler BEFORE starting the harness
        testHarness.Bus.ConnectReceiveEndpoint(
            "test-response-endpoint",
            endpoint =>
            {
                endpoint.Handler<GetNutritionGoalsByUserIdRequest>(async context =>
                {
                    await context.RespondAsync(
                        new GetNutritionGoalsByUserIdResponse(
                            new NutritionGoals(2000, 500, 250, 180),
                            []
                        )
                    );
                });
            }
        );

        await testHarness.Start();

        // Add an ingredient to the recipe using faker
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.Recipes.AddAsync(recipe);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Create recipe diary entry using faker
        var diaryRequest = new AddRecipeDiaryRequest(
            RecipeId: recipe.Id,
            MealType: MealTypes.Breakfast,
            Quantity: 2f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow),
            ServingSizeId: recipe.ServingSizes[0].Id
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/recipe-diaries", diaryRequest);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify daily nutrition overview was created and updated
        var dailyNutritionOverview =
            await _nutritionWriteDbContext.DailyNutritionOverviews.FirstOrDefaultAsync(d =>
                d.UserId == _user.Id && d.Date == diaryRequest.EntryDate
            );

        dailyNutritionOverview.Should().NotBeNull();
        dailyNutritionOverview!
            .NutritionalContent.Energy.Value.Should()
            .BeApproximately(recipe.NutritionalContents.Energy.Value / 6 * 2f, 0.1f);
        dailyNutritionOverview
            .NutritionalContent.Protein.Should()
            .BeApproximately(recipe.NutritionalContents.Protein / 6 * 2f, 0.1f);
        dailyNutritionOverview
            .NutritionalContent.Fat.Should()
            .BeApproximately(recipe.NutritionalContents.Fat / 6 * 2f, 0.1f);
        dailyNutritionOverview
            .NutritionalContent.Carbohydrates.Should()
            .BeApproximately(recipe.NutritionalContents.Carbohydrates / 6 * 2f, 0.1f);
    }

    [Fact]
    public async Task DeleteRecipeDiary_ShouldUpdateDailyNutritionOverview()
    {
        // Arrange

        var foodId = FoodId.NewId();
        var servingSize = ServingSizeFaker.Generate();

        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        var ingredient = IngredientFaker.Generate(foodId: foodId, servingSizeId: servingSize.Id);
        var recipe = RecipeFaker.Generate(ingredients: [ingredient]);

        var overview = DailyNutritionOverviewFaker.Generate(
            userId: _user.Id,
            date: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        await _nutritionWriteDbContext.DailyNutritionOverviews.AddAsync(overview);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.Ingredients.AddAsync(ingredient);
        await _nutritionWriteDbContext.Recipes.AddAsync(recipe);
        await _nutritionWriteDbContext.SaveChangesAsync();

        _nutritionWriteDbContext.ChangeTracker.Clear();

        // Create recipe diary entry using faker
        var diaryRequest = new AddRecipeDiaryRequest(
            RecipeId: recipe.Id,
            MealType: MealTypes.Breakfast,
            Quantity: 1.5f,
            EntryDate: DateOnly.FromDateTime(DateTime.UtcNow),
            ServingSizeId: recipe.ServingSizes[0].Id
        );

        var addResponse = await _client.PostAsJsonAsync("/api/recipe-diaries", diaryRequest);
        var addResult = await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Act
        var deleteResponse = await _client.DeleteAsync($"/api/recipe-diaries/{addResult.id}");

        // Assert
        await deleteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify daily nutrition overview was updated
        var dailyNutritionOverview =
            await _nutritionWriteDbContext.DailyNutritionOverviews.FirstOrDefaultAsync(d =>
                d.UserId == _user.Id && d.Date == diaryRequest.EntryDate
            );

        dailyNutritionOverview.Should().NotBeNull();
        dailyNutritionOverview!
            .NutritionalContent.Energy.Value.Should()
            .BeApproximately(overview.NutritionalContent.Energy.Value, 0.1f);
        dailyNutritionOverview
            .NutritionalContent.Protein.Should()
            .BeApproximately(overview.NutritionalContent.Protein, 0.1f);
        dailyNutritionOverview
            .NutritionalContent.Fat.Should()
            .BeApproximately(overview.NutritionalContent.Fat, 0.1f);
        dailyNutritionOverview
            .NutritionalContent.Carbohydrates.Should()
            .BeApproximately(overview.NutritionalContent.Carbohydrates, 0.1f);
    }
}
