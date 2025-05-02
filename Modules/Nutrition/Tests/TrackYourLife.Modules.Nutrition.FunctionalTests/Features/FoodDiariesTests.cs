using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.FunctionalTests.Utils;
using TrackYourLife.Modules.Nutrition.Presentation.Contracts;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;
using TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries.Queries;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests.Features;

[Collection("Nutrition Integration Tests")]
public class FoodDiariesTests(NutritionFunctionalTestWebAppFactory factory)
    : NutritionBaseIntegrationTest(factory)
{
    [Fact]
    public async Task AddFoodDiary_WithValidData_ShouldCreateEntry()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var foodId = FoodId.NewId();
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var request = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1.5f,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var response = await _client.PostAsJsonAsync(ApiRoutes.FoodDiaries, request);

        // Assert
        var content = await response.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        content.Should().NotBeNull();
        content.id.Should().NotBe(Guid.Empty);

        var foodDiary = await _nutritionWriteDbContext.FoodDiaries.FindAsync(
            NutritionDiaryId.Create(content.id)
        );
        foodDiary.Should().NotBeNull();
        foodDiary!.FoodId.Should().Be(foodId);
        foodDiary.MealType.Should().Be(MealTypes.Breakfast);
        foodDiary.ServingSizeId.Should().Be(servingSize.Id);
        foodDiary.Quantity.Should().Be(1.5f);
        foodDiary.Date.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task UpdateFoodDiary_WithValidData_ShouldUpdateEntry()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var foodId = FoodId.NewId();
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var addRequest = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1.5f,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.FoodDiaries, addRequest);
        addResponse.EnsureSuccessStatusCode();

        var addResult = await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        var updateRequest = new UpdateFoodDiaryRequest(
            NutritionDiaryId.Create(addResult!.id),
            2.0f,
            servingSize.Id,
            MealTypes.Lunch
        );

        // Act
        var response = await _client.PutAsJsonAsync(ApiRoutes.FoodDiaries, updateRequest);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        var foodDiary = await _nutritionWriteDbContext.FoodDiaries.FindAsync(
            NutritionDiaryId.Create(addResult.id)
        );
        foodDiary.Should().NotBeNull();
        foodDiary!.FoodId.Should().Be(foodId);
        foodDiary.MealType.Should().Be(MealTypes.Lunch);
        foodDiary.ServingSizeId.Should().Be(servingSize.Id);
        foodDiary.Quantity.Should().Be(2.0f);
        foodDiary.Date.Should().Be(DateOnly.FromDateTime(DateTime.UtcNow));
    }

    [Fact]
    public async Task DeleteFoodDiary_WithValidId_ShouldDeleteEntry()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var foodId = FoodId.NewId();
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var addRequest = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1.5f,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.FoodDiaries, addRequest);
        addResponse.EnsureSuccessStatusCode();
        var addResult = await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        // Act
        var response = await _client.DeleteAsync($"{ApiRoutes.FoodDiaries}/{addResult!.id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetFoodDiaryById_WithValidId_ShouldReturnEntry()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var foodId = FoodId.NewId();
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var addRequest = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1.5f,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.FoodDiaries, addRequest);
        addResponse.EnsureSuccessStatusCode();
        var addResult = await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        // Act
        var response = await _client.GetAsync($"{ApiRoutes.FoodDiaries}/{addResult.id}");

        // Assert
        var result = await response.ShouldHaveStatusCodeAndContent<FoodDiaryDto>(HttpStatusCode.OK);

        result.Should().NotBeNull();
        result.Id.Value.ToString().Should().Be(addResult.id.ToString());
        result.Food.Id.Should().Be(food.Id);
        result.ServingSize.Id.Should().Be(servingSize.Id);
        result.Quantity.Should().Be(1.5f);
        result.MealType.Should().Be(MealTypes.Breakfast);
    }

    [Fact]
    public async Task GetFoodDiaryById_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync(
            $"{ApiRoutes.FoodDiaries}/{NutritionDiaryId.NewId().Value}"
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetNutritionDiariesByDate_WithValidData_ShouldReturnEntries()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var foodId = FoodId.NewId();
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1.5f,
            date
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.FoodDiaries, request);
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
        content
            .Diaries[MealTypes.Breakfast][0]
            .NutritionalContents.Energy.Value.Should()
            .Be(food.NutritionalContents.Energy.Value * 1.5f * servingSize.NutritionMultiplier);
        diary.Quantity.Should().Be(1.5f);
        diary.MealType.Should().Be(MealTypes.Breakfast);
        diary.Date.Should().Be(date);
    }

    [Fact]
    public async Task GetNutritionOverviewByPeriod_WithValidData_ShouldReturnTotal()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var foodId = FoodId.NewId();
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1.5f,
            date
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.FoodDiaries, request);
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
    public async Task AddFoodDiary_ShouldUpdateDailyNutritionOverview()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate(nutritionMultiplier: 2.0f);
        var foodId = FoodId.NewId();
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var request = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1.5f,
            date
        );

        // Act
        var response = await _client.PostAsJsonAsync(ApiRoutes.FoodDiaries, request);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify daily nutrition overview was created and updated
        var dailyNutritionOverview =
            await _nutritionWriteDbContext.DailyNutritionOverviews.FirstOrDefaultAsync(d =>
                d.UserId == _user.Id && d.Date == date
            );

        dailyNutritionOverview.Should().NotBeNull();

        dailyNutritionOverview!
            .NutritionalContent.Energy.Value.Should()
            .Be(food.NutritionalContents.Energy.Value * 1.5f * servingSize.NutritionMultiplier);
        dailyNutritionOverview
            .NutritionalContent.Protein.Should()
            .Be(food.NutritionalContents.Protein * 1.5f * servingSize.NutritionMultiplier);
        dailyNutritionOverview
            .NutritionalContent.Fat.Should()
            .Be(food.NutritionalContents.Fat * 1.5f * servingSize.NutritionMultiplier);
        dailyNutritionOverview
            .NutritionalContent.Carbohydrates.Should()
            .Be(food.NutritionalContents.Carbohydrates * 1.5f * servingSize.NutritionMultiplier);
    }

    [Fact]
    public async Task DeleteFoodDiary_ShouldUpdateDailyNutritionOverview()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate(nutritionMultiplier: 2.0f);
        var foodId = FoodId.NewId();
        var food = FoodFaker.Generate(
            id: foodId,
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var addRequest = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1.5f,
            date
        );

        var addResponse = await _client.PostAsJsonAsync(ApiRoutes.FoodDiaries, addRequest);
        var addResult = await addResponse.ShouldHaveStatusCodeAndContent<TestIdResponse>(
            HttpStatusCode.Created
        );

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Act
        var deleteResponse = await _client.DeleteAsync($"{ApiRoutes.FoodDiaries}/{addResult!.id}");

        // Assert
        await deleteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify daily nutrition overview was updated
        var dailyNutritionOverview =
            await _nutritionWriteDbContext.DailyNutritionOverviews.FirstOrDefaultAsync(d =>
                d.UserId == _user.Id && d.Date == date
            );

        dailyNutritionOverview.Should().NotBeNull();
        dailyNutritionOverview!.NutritionalContent.Energy.Value.Should().Be(0);
        dailyNutritionOverview.NutritionalContent.Protein.Should().Be(0);
        dailyNutritionOverview.NutritionalContent.Fat.Should().Be(0);
        dailyNutritionOverview.NutritionalContent.Carbohydrates.Should().Be(0);
    }
}
