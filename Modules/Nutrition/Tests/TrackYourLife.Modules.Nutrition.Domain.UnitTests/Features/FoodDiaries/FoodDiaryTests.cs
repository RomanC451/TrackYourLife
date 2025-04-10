using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.FoodDiaries;

public class FoodDiaryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateFoodDiary()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var foodDiary = FoodDiary
            .Create(id, userId, foodId, quantity, date, mealType, servingSizeId)
            .Value;

        // Assert
        foodDiary.Should().NotBeNull();
        foodDiary.Id.Should().Be(id);
        foodDiary.UserId.Should().Be(userId);
        foodDiary.FoodId.Should().Be(foodId);
        foodDiary.Quantity.Should().Be(quantity);
        foodDiary.Date.Should().Be(date);
        foodDiary.MealType.Should().Be(mealType);
        foodDiary.ServingSizeId.Should().Be(servingSizeId);
    }

    [Fact]
    public void Create_WithInvalidId_ShouldFail()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(
            NutritionDiaryId.Empty,
            userId,
            foodId,
            quantity,
            date,
            mealType,
            servingSizeId
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
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(
            id,
            UserId.Empty,
            foodId,
            quantity,
            date,
            mealType,
            servingSizeId
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("UserId");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidFoodId_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(
            id,
            userId,
            FoodId.Empty,
            quantity,
            date,
            mealType,
            servingSizeId
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("FoodId");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidQuantity_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(id, userId, foodId, -1, date, mealType, servingSizeId);

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
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(
            id,
            userId,
            foodId,
            quantity,
            DateOnly.MinValue,
            mealType,
            servingSizeId
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Date");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidServingSizeId_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        // Act
        var result = FoodDiary.Create(
            id,
            userId,
            foodId,
            quantity,
            date,
            mealType,
            ServingSizeId.Empty
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("ServingSizeId");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
    }

    [Fact]
    public void Create_WithInvalidMealType_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        // Act
        var result = FoodDiary.Create(
            id,
            userId,
            foodId,
            quantity,
            date,
            (MealTypes)100,
            servingSizeId
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
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, foodId, quantity, date, mealType, servingSizeId)
            .Value;

        var newQuantity = 200;

        // Act
        var result = foodDiary.UpdateQuantity(newQuantity);

        // Assert
        result.IsSuccess.Should().BeTrue();
        foodDiary.Quantity.Should().Be(newQuantity);
    }

    [Fact]
    public void UpdateQuantity_WithInvalidQuantity_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, foodId, quantity, date, mealType, servingSizeId)
            .Value;

        var newQuantity = -1;

        // Act
        var result = foodDiary.UpdateQuantity(newQuantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("NotPositive");
        result.Error.Code.Should().Contain("Quantity");
        result.Error.Code.Should().Contain(nameof(NutritionDiary));
        foodDiary.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void UpdateMealType_WithValidMealType_ShouldUpdateMealType()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, foodId, quantity, date, mealType, servingSizeId)
            .Value;

        var newMealType = MealTypes.Dinner;

        // Act
        var result = foodDiary.UpdateMealType(newMealType);

        // Assert
        result.IsSuccess.Should().BeTrue();
        foodDiary.MealType.Should().Be(newMealType);
    }

    [Fact]
    public void UpdateMealType_WithInvalidMealType_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, foodId, quantity, date, mealType, servingSizeId)
            .Value;

        var newMealType = (MealTypes)100;

        // Act
        var result = foodDiary.UpdateMealType(newMealType);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Invalid");
        result.Error.Code.Should().Contain("MealType");
        result.Error.Code.Should().Contain(nameof(NutritionDiary));
        foodDiary.MealType.Should().Be(mealType);
    }

    [Fact]
    public void UpdateServingSizeId_WithValidServingSizeId_ShouldUpdateServingSizeId()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId1 = ServingSizeId.NewId();
        var servingSizeId2 = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, foodId, quantity, date, mealType, servingSizeId1)
            .Value;

        // Act
        var result = foodDiary.UpdateServingSizeId(servingSizeId2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        foodDiary.ServingSizeId.Should().Be(servingSizeId2);
    }

    [Fact]
    public void UpdateServingSizeId_WithInvalidServingSizeId_ShouldFail()
    {
        // Arrange
        var id = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId1 = ServingSizeId.NewId();
        var quantity = 100;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var mealType = MealTypes.Lunch;

        var foodDiary = FoodDiary
            .Create(id, userId, foodId, quantity, date, mealType, servingSizeId1)
            .Value;

        // Act
        var result = foodDiary.UpdateServingSizeId(ServingSizeId.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("ServingSizeId");
        result.Error.Code.Should().Contain(nameof(FoodDiary));
        foodDiary.ServingSizeId.Should().Be(servingSizeId1);
    }
}
