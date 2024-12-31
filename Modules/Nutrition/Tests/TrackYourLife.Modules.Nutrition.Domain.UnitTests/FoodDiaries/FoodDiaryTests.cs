using FluentAssertions;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.FoodDiaries;

public class FoodDiaryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateFoodDiary()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, servingSize)
            .Value;

        // Assert
        foodDiary.Should().NotBeNull();
        foodDiary.Id.Should().Be(id);
        foodDiary.UserId.Should().Be(userId);
        foodDiary.Food.Should().Be(food);
        foodDiary.Quantity.Should().Be(quantity);
        foodDiary.Date.Should().Be(date);
        foodDiary.MealType.Should().Be(mealType);
        foodDiary.ServingSize.Should().Be(servingSize);
    }

    [Fact]
    public void Create_WithInvalidId_ShouldFail()
    {
        // Arrange
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(
            NutritionDiaryId.Empty,
            userId,
            food,
            quantity,
            date,
            mealType,
            servingSize
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Id");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidUserId_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(
            id,
            UserId.Empty,
            food,
            quantity,
            date,
            mealType,
            servingSize
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("UserId");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidFood_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(id, userId, null!, quantity, date, mealType, servingSize);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Null");
        result.Error.Code.Should().Contain("Food");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidQuantity_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(id, userId, food, -1, date, mealType, servingSize);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");
        result.Error.Code.Should().Contain("Quantity");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidDate_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(
            id,
            userId,
            food,
            quantity,
            DateOnly.MinValue,
            mealType,
            servingSize
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Date");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidServingSize_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(id, userId, food, quantity, date, mealType, null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("ServingSize");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidMealType_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = FoodDiary.Create(
            id,
            userId,
            food,
            quantity,
            date,
            (MealTypes)100,
            servingSize
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Invalid");
        result.Error.Code.Should().Contain("MealType");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_ShouldUpdateQuantity()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, servingSize)
            .Value;

        // Act
        var result = foodDiary.UpdateQuantity(200);

        // Assert
        result.IsSuccess.Should().BeTrue();
        foodDiary.Quantity.Should().Be(200);
    }

    [Fact]
    public void UpdateQuantity_WithInvalidQuantity_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, servingSize)
            .Value;

        // Act
        var result = foodDiary.UpdateQuantity(-1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");
        result.Error.Code.Should().Contain("Quantity");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void UpdateQuantity_WithZeroQuantity_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, servingSize)
            .Value;

        // Act
        var result = foodDiary.UpdateQuantity(0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");
        result.Error.Code.Should().Contain("Quantity");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void UpdateServingSize_WithValidServingSize_ShouldUpdateServingSize()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize1 = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;
        var servingSize2 = ServingSize.Create(ServingSizeId.NewId(), 2, "ml", 200).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [
                FoodServingSize.Create(foodId, servingSize1.Id, servingSize1, 0).Value,
                FoodServingSize.Create(foodId, servingSize2.Id, servingSize2, 0).Value
            ]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, servingSize1)
            .Value;

        // Act
        var result = foodDiary.UpdateServingSizeId(servingSize2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        foodDiary.ServingSize.Should().Be(servingSize2);
    }

    [Fact]
    public void UpdateServingSize_WithInvalidServingSize_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize1 = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;
        var servingSize2 = ServingSize.Create(ServingSizeId.NewId(), 2, "ml", 200).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [
                FoodServingSize.Create(foodId, servingSize1.Id, servingSize1, 0).Value,
                FoodServingSize.Create(foodId, servingSize2.Id, servingSize2, 0).Value
            ]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, servingSize1)
            .Value;

        // Act
        var result = foodDiary.UpdateServingSizeId((ServingSize)null!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Null");
        result.Error.Code.Should().Contain("ServingSize");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void UpdateServingSize_WithNotExistingServingSizeInFood_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();

        var oldServingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;
        var newServingSize = ServingSize.Create(ServingSizeId.NewId(), 2, "ml", 200).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, oldServingSize.Id, oldServingSize, 0).Value,]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, oldServingSize)
            .Value;

        // Act
        var result = foodDiary.UpdateServingSizeId(newServingSize);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotFound");
        result.Error.Code.Should().Contain("Food");
        result.Error.Code.Should().Contain(nameof(Food));
    }

    [Fact]
    public void UpdateMealType_WithValidMealType_ShouldUpdateMealType()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, servingSize)
            .Value;

        // Act
        var result = foodDiary.UpdateMealType(MealTypes.Dinner);

        // Assert
        result.IsSuccess.Should().BeTrue();
        foodDiary.MealType.Should().Be(MealTypes.Dinner);
    }

    [Fact]
    public void UpdateMealType_WithInvalidMealType_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();

        var foodId = FoodId.NewId();

        var servingSize = ServingSize.Create(ServingSizeId.NewId(), 1, "ml", 100).Value;

        var food = Food.Create(
            foodId,
            "Food",
            "Kaufland",
            "R0",
            "Lapte",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSize.Id, servingSize, 0).Value]
        ).Value;

        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, food, quantity, date, mealType, servingSize)
            .Value;

        // Act
        var result = foodDiary.UpdateMealType((MealTypes)100);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Invalid");
        result.Error.Code.Should().Contain("MealType");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }
}
