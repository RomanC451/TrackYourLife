using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries.DomainEvents;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.RecipeDiaries;

public class RecipeDiaryTests
{
    private readonly NutritionDiaryId _id;
    private readonly UserId _userId;
    private readonly RecipeId _recipeId;
    private readonly float _quantity;
    private readonly DateOnly _date;
    private readonly MealTypes _mealType;

    public RecipeDiaryTests()
    {
        _id = NutritionDiaryId.NewId();
        _userId = UserId.NewId();
        _recipeId = RecipeId.NewId();
        _quantity = 100f;
        _date = DateOnly.FromDateTime(DateTime.UtcNow);
        _mealType = MealTypes.Breakfast;
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateRecipeDiary()
    {
        // Act
        var result = RecipeDiary.Create(_id, _userId, _recipeId, _quantity, _date, _mealType);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.UserId.Should().Be(_userId);
        result.Value.RecipeId.Should().Be(_recipeId);
        result.Value.Quantity.Should().Be(_quantity);
        result.Value.Date.Should().Be(_date);
        result.Value.MealType.Should().Be(_mealType);
        result.Value.CreatedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.Value.ModifiedOnUtc.Should().BeNull();

        // Verify domain event
        var domainEvent =
            result.Value.GetDirectDomainEvents().Should().ContainSingle().Subject
            as RecipeDiaryCreatedDomainEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(_userId);
        domainEvent.RecipeId.Should().Be(_recipeId);
        domainEvent.Date.Should().Be(_date);
        domainEvent.Quantity.Should().Be(_quantity);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        // Arrange
        var emptyId = NutritionDiaryId.Empty;

        // Act
        var result = RecipeDiary.Create(emptyId, _userId, _recipeId, _quantity, _date, _mealType);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(RecipeDiary)}.{nameof(RecipeDiary.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        // Arrange
        var emptyUserId = UserId.Empty;

        // Act
        var result = RecipeDiary.Create(_id, emptyUserId, _recipeId, _quantity, _date, _mealType);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(RecipeDiary)}.{nameof(RecipeDiary.UserId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyRecipeId_ShouldFail()
    {
        // Arrange
        var emptyRecipeId = RecipeId.Empty;

        // Act
        var result = RecipeDiary.Create(_id, _userId, emptyRecipeId, _quantity, _date, _mealType);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(RecipeDiary)}.{nameof(RecipeDiary.RecipeId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNegativeQuantity_ShouldFail()
    {
        // Arrange
        var negativeQuantity = -100f;

        // Act
        var result = RecipeDiary.Create(
            _id,
            _userId,
            _recipeId,
            negativeQuantity,
            _date,
            _mealType
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be(
                $"{nameof(RecipeDiary)}.{nameof(RecipeDiary.Quantity).ToCapitalCase()}.NotPositive"
            );
    }

    [Fact]
    public void Create_WithInvalidMealType_ShouldFail()
    {
        // Arrange
        var invalidMealType = (MealTypes)999;

        // Act
        var result = RecipeDiary.Create(_id, _userId, _recipeId, _quantity, _date, invalidMealType);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(RecipeDiary)}.{nameof(RecipeDiary.MealType).ToCapitalCase()}.Invalid");
    }

    [Fact]
    public void OnDelete_ShouldRaiseDomainEvent()
    {
        // Arrange
        var recipeDiary = RecipeDiary
            .Create(_id, _userId, _recipeId, _quantity, _date, _mealType)
            .Value;
        recipeDiary.ClearDirectDomainEvents(); // Clear the creation event

        // Act
        recipeDiary.OnDelete();

        // Assert
        var domainEvent =
            recipeDiary.GetDirectDomainEvents().Should().ContainSingle().Subject
            as RecipeDiaryDeletedDomainEvent;
        domainEvent.Should().NotBeNull();
        domainEvent!.UserId.Should().Be(_userId);
        domainEvent.RecipeId.Should().Be(_recipeId);
        domainEvent.Date.Should().Be(_date);
        domainEvent.Quantity.Should().Be(_quantity);
    }
}
