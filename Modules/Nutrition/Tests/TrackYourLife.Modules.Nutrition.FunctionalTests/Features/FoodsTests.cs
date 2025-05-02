using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.FunctionalTests.Utils;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests.Features;

public record PagedListResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public bool HasPreviousPage { get; init; }
    public int MaxPage { get; init; }
    public bool HasNextPage { get; init; }
}

[Collection("Nutrition Integration Tests")]
public class FoodsTests(NutritionFunctionalTestWebAppFactory factory)
    : NutritionBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetFoodById_WithValidId_ShouldReturnFood()
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

        // Act
        var response = await _client.GetAsync($"/api/foods/{food.Id.Value}");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
        var foodDto = await response.Content.ReadFromJsonAsync<FoodDto>();
        foodDto.Should().NotBeNull();
        foodDto!.Name.Should().Be(food.Name);
        foodDto.BrandName.Should().Be(food.BrandName);
        foodDto.Type.Should().Be(food.Type);
        foodDto.ServingSizes.Should().HaveCount(food.FoodServingSizes.Count);
    }

    [Fact]
    public async Task GetFoodById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/foods/{nonExistingId}");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SearchFoodsByName_WithValidSearchParam_ShouldReturnFoods()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var food = FoodFaker.Generate(
            name: "Test1 Food",
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        SetupFoodApiMock("Test1", FoodApiResponseFaker.Generate(1, "Test1"));

        // Act
        var response = await _client.GetAsync("/api/foods/search?searchParam=Test1");

        // Assert
        var content = await response.ShouldHaveStatusCodeAndContent<PagedListResponse<FoodDto>>(
            HttpStatusCode.OK
        );
        content.Items.Should().HaveCount(1);
        content.Items.First().Name.Should().Be(food.Name);
    }

    [Fact]
    public async Task SearchFoodsByName_WithEmptySearchParam_ShouldReturnBadRequest()
    {
        // Act
        var response = await _client.GetAsync("/api/foods/search?searchParam=");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SearchFoodsByName_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        // Create 15 foods
        for (int i = 0; i < 15; i++)
        {
            var servingSize = ServingSizeFaker.Generate();
            var food = FoodFaker.Generate(
                name: $"Test2 Food {i}",
                foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
            );
            await _nutritionWriteDbContext.Foods.AddAsync(food);
            await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        }
        await _nutritionWriteDbContext.SaveChangesAsync();

        SetupFoodApiMock("Test2", FoodApiResponseFaker.Generate(15));

        // Act
        var response = await _client.GetAsync(
            "/api/foods/search?searchParam=Test2&page=2&pageSize=10"
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
        var pagedList = await response.Content.ReadFromJsonAsync<PagedListResponse<FoodDto>>();
        pagedList.Should().NotBeNull();
        pagedList!.Items.Should().HaveCount(5); // Only 5 items on second page
        pagedList.MaxPage.Should().Be(2); // Total pages
        pagedList.Page.Should().Be(2);
        pagedList.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task UpdateFoodApiCookies_WithValidFile_ShouldUpdateCookies()
    {
        // Arrange
        var cookieFileContent =
            @"# Netscape HTTP Cookie File
# https://curl.haxx.se/rfc/cookie_spec.html
# This is a generated file!  Do not edit.

.example.com	TRUE	/	FALSE	1735689600	sessionId	abc123
.example.com	TRUE	/	TRUE	1735689600	secureCookie	xyz789
.example.com	TRUE	/api	FALSE	0	persistentCookie	def456";

        var cookieFileBytes = System.Text.Encoding.UTF8.GetBytes(cookieFileContent);
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(cookieFileBytes);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
            "text/plain"
        );
        content.Add(fileContent, "cookieFile", "cookies.txt");

        // Act
        var response = await _client.PutAsync("/api/foods/food-api-cookies", content);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateFoodApiCookies_WithInvalidFile_ShouldReturnBadRequest()
    {
        // Arrange
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 1, 2, 3 });
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(
            "application/json"
        );
        content.Add(fileContent, "invalidField", "cookies.json");

        // Act
        var response = await _client.PutAsync("/api/foods/food-api-cookies", content);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateFoodDiary_ShouldAddFoodToHistory()
    {
        // Arrange
        var servingSize = ServingSizeFaker.Generate();
        var food = FoodFaker.Generate(
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );
        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.SaveChangesAsync();

        var createFoodDiaryRequest = new AddFoodDiaryRequest(
            food.Id,
            MealTypes.Breakfast,
            servingSize.Id,
            1,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/food-diaries", createFoodDiaryRequest);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Verify food was added to history
        var foodHistory = await _nutritionWriteDbContext.FoodHistories.FirstOrDefaultAsync(fh =>
            fh.UserId == _user.Id && fh.FoodId == food.Id
        );
        foodHistory.Should().NotBeNull();
        foodHistory!.UserId.Should().Be(_user.Id);
        foodHistory.FoodId.Should().Be(food.Id);
    }
}
