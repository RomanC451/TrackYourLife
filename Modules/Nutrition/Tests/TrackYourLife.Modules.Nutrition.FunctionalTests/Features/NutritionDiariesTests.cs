using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.FunctionalTests.Utils;
using TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries.Queries;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests.Features;

[Collection("Nutrition Integration Tests")]
public class NutritionDiariesTests(NutritionFunctionalTestWebAppFactory factory)
    : NutritionBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetNutritionDiariesByDate_WithMixedDiaries_ShouldReturnGroupedDiaries()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Create food diary for breakfast
        var servingSize = ServingSizeFaker.Generate();
        var food = FoodFaker.Generate(
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );
        var foodDiary = FoodDiaryFaker.Generate(
            userId: _user.Id,
            foodId: food.Id,
            servingSizeId: servingSize.Id,
            date: date,
            mealType: MealTypes.Breakfast
        );

        // Create recipe diary for lunch
        var recipe = RecipeFaker.Generate(
            ingredients: [IngredientFaker.Generate(foodId: food.Id, servingSizeId: servingSize.Id)]
        );
        var recipeDiary = RecipeDiaryFaker.Generate(
            userId: _user.Id,
            recipeId: recipe.Id,
            date: date,
            mealType: MealTypes.Lunch
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.Recipes.AddAsync(recipe);
        await _nutritionWriteDbContext.FoodDiaries.AddAsync(foodDiary);
        await _nutritionWriteDbContext.RecipeDiaries.AddAsync(recipeDiary);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/nutrition-diaries/{date:yyyy-MM-dd}");

        // Assert
        var result =
            await response.ShouldHaveStatusCodeAndContent<GetNutritionDiariesByDateResponse>(
                HttpStatusCode.OK
            );
        result.Should().NotBeNull();
        result!.Diaries[MealTypes.Breakfast].Should().HaveCount(1);
        result!.Diaries[MealTypes.Lunch].Should().HaveCount(1);
        result!.Diaries[MealTypes.Snacks].Should().BeEmpty();
        result!.Diaries[MealTypes.Dinner].Should().BeEmpty();

        // Check breakfast entries
        result.Diaries[MealTypes.Breakfast].Should().HaveCount(1);
        var breakfastDiary = result.Diaries[MealTypes.Breakfast][0];
        breakfastDiary.Id.Should().Be(foodDiary.Id);
        breakfastDiary.DiaryType.Should().Be(DiaryType.FoodDiary);

        // Check lunch entries
        result.Diaries[MealTypes.Lunch].Should().HaveCount(1);
        var lunchDiary = result.Diaries[MealTypes.Lunch][0];
        lunchDiary.Id.Should().Be(recipeDiary.Id);
        lunchDiary.DiaryType.Should().Be(DiaryType.RecipeDiary);
    }

    [Fact]
    public async Task GetNutritionDiariesByDate_WithNoDiaries_ShouldReturnEmptyDictionary()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var response = await _client.GetAsync($"/api/nutrition-diaries/{date:yyyy-MM-dd}");

        // Assert
        var result =
            await response.ShouldHaveStatusCodeAndContent<GetNutritionDiariesByDateResponse>(
                HttpStatusCode.OK
            );
        result.Should().NotBeNull();
        result.Diaries[MealTypes.Breakfast].Should().BeEmpty();
        result.Diaries[MealTypes.Lunch].Should().BeEmpty();
        result.Diaries[MealTypes.Snacks].Should().BeEmpty();
        result.Diaries[MealTypes.Dinner].Should().BeEmpty();
    }

    [Fact]
    public async Task GetNutritionOverviewByPeriod_WithValidDiaries_ShouldReturnTotalNutritionalContent()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        // Create food diary for day 1
        var servingSize = ServingSizeFaker.Generate(nutritionMultiplier: 2.0f);
        var food = FoodFaker.Generate(
            foodServingSizes: [FoodServingSizeFaker.Generate(0, servingSize: servingSize)]
        );
        var foodDiary = FoodDiaryFaker.Generate(
            userId: _user.Id,
            foodId: food.Id,
            servingSizeId: servingSize.Id,
            date: startDate
        );

        // Create recipe diary for day 2
        var recipe = RecipeFaker.Generate(
            ingredients: [IngredientFaker.Generate(foodId: food.Id, servingSizeId: servingSize.Id)]
        );
        var recipeDiary = RecipeDiaryFaker.Generate(
            userId: _user.Id,
            recipeId: recipe.Id,
            date: startDate.AddDays(1)
        );

        await _nutritionWriteDbContext.Foods.AddAsync(food);
        await _nutritionWriteDbContext.ServingSizes.AddAsync(servingSize);
        await _nutritionWriteDbContext.Recipes.AddAsync(recipe);
        await _nutritionWriteDbContext.FoodDiaries.AddAsync(foodDiary);
        await _nutritionWriteDbContext.RecipeDiaries.AddAsync(recipeDiary);
        await _nutritionWriteDbContext.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync(
            $"/api/nutrition-diaries/nutrition-overview?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}"
        );

        // Assert
        var nutritionalContent = await response.ShouldHaveStatusCodeAndContent<NutritionalContent>(
            HttpStatusCode.OK
        );
        nutritionalContent.Should().NotBeNull();
        nutritionalContent!
            .Energy.Value.Should()
            .BeApproximately(
                food.NutritionalContents.Energy.Value
                    * servingSize.NutritionMultiplier
                    * foodDiary.Quantity
                    + recipe.NutritionalContents.Energy.Value
                        * (1 / recipe.Portions)
                        * recipeDiary.Quantity,
                0.1f
            );
        nutritionalContent
            .Protein.Should()
            .BeApproximately(
                food.NutritionalContents.Protein
                    * servingSize.NutritionMultiplier
                    * foodDiary.Quantity
                    + recipe.NutritionalContents.Protein
                        * (1 / recipe.Portions)
                        * recipeDiary.Quantity,
                0.1f
            );
        nutritionalContent
            .Fat.Should()
            .BeApproximately(
                food.NutritionalContents.Fat * servingSize.NutritionMultiplier * foodDiary.Quantity
                    + recipe.NutritionalContents.Fat * (1 / recipe.Portions) * recipeDiary.Quantity,
                0.1f
            );
        nutritionalContent
            .Carbohydrates.Should()
            .BeApproximately(
                food.NutritionalContents.Carbohydrates
                    * servingSize.NutritionMultiplier
                    * foodDiary.Quantity
                    + recipe.NutritionalContents.Carbohydrates
                        * (1 / recipe.Portions)
                        * recipeDiary.Quantity,
                0.1f
            );
    }

    [Fact]
    public async Task GetNutritionOverviewByPeriod_WithNoDiaries_ShouldReturnZeroNutritionalContent()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        // Act
        var response = await _client.GetAsync(
            $"/api/nutrition-diaries/nutrition-overview?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}"
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.OK);
        var nutritionalContent = await response.Content.ReadFromJsonAsync<NutritionalContent>();
        nutritionalContent.Should().NotBeNull();
        nutritionalContent!.Energy.Value.Should().Be(0);
        nutritionalContent.Protein.Should().Be(0);
        nutritionalContent.Fat.Should().Be(0);
        nutritionalContent.Carbohydrates.Should().Be(0);
    }
}
