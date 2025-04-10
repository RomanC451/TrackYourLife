using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.NutritionDiaries;

// Concrete implementation for testing
public class TestNutritionDiary : NutritionDiary
{
    private TestNutritionDiary()
        : base() { }

    private TestNutritionDiary(
        NutritionDiaryId id,
        UserId userId,
        float quantity,
        DateOnly date,
        MealTypes mealType
    )
        : base(id, userId, quantity, date, mealType) { }

    public static Result<TestNutritionDiary> Create(
        NutritionDiaryId id,
        UserId userId,
        float quantity,
        DateOnly date,
        MealTypes mealType
    )
    {
        return Result.Success(new TestNutritionDiary(id, userId, quantity, date, mealType));
    }
}

public class NutritionDiaryTests
{
    private readonly NutritionDiaryId _id;
    private readonly UserId _userId;
    private readonly float _quantity;
    private readonly DateOnly _date;
    private readonly MealTypes _mealType;

    public NutritionDiaryTests()
    {
        _id = NutritionDiaryId.NewId();
        _userId = UserId.NewId();
        _quantity = 100f;
        _date = DateOnly.FromDateTime(DateTime.UtcNow);
        _mealType = MealTypes.Breakfast;
    }

    [Fact]
    public void UpdateQuantity_WithValidValue_ShouldUpdate()
    {
        // Arrange
        var diary = TestNutritionDiary.Create(_id, _userId, _quantity, _date, _mealType).Value;
        var newQuantity = 200f;

        // Act
        var result = diary.UpdateQuantity(newQuantity);

        // Assert
        result.IsSuccess.Should().BeTrue();
        diary.Quantity.Should().Be(newQuantity);
        diary.ModifiedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateQuantity_WithNegativeValue_ShouldFail()
    {
        // Arrange
        var diary = TestNutritionDiary.Create(_id, _userId, _quantity, _date, _mealType).Value;
        var negativeQuantity = -100f;

        // Act
        var result = diary.UpdateQuantity(negativeQuantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be(
                $"{nameof(NutritionDiary)}.{nameof(NutritionDiary.Quantity).ToCapitalCase()}.NotPositive"
            );
        diary.Quantity.Should().Be(_quantity);
        diary.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void UpdateMealType_WithValidValue_ShouldUpdate()
    {
        // Arrange
        var diary = TestNutritionDiary.Create(_id, _userId, _quantity, _date, _mealType).Value;
        var newMealType = MealTypes.Dinner;

        // Act
        var result = diary.UpdateMealType(newMealType);

        // Assert
        result.IsSuccess.Should().BeTrue();
        diary.MealType.Should().Be(newMealType);
        diary.ModifiedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateMealType_WithInvalidValue_ShouldFail()
    {
        // Arrange
        var diary = TestNutritionDiary.Create(_id, _userId, _quantity, _date, _mealType).Value;
        var invalidMealType = (MealTypes)999; // Invalid enum value

        // Act
        var result = diary.UpdateMealType(invalidMealType);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be(
                $"{nameof(NutritionDiary)}.{nameof(NutritionDiary.MealType).ToCapitalCase()}.Invalid"
            );
        diary.MealType.Should().Be(_mealType);
        diary.ModifiedOnUtc.Should().BeNull();
    }
}
