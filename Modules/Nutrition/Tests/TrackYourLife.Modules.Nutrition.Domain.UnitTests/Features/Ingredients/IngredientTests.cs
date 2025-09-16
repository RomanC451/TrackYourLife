using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Features.Ingredients;

public class IngredientTests
{
    private readonly UserId _userId;
    private readonly IngredientId _id;
    private readonly FoodId _foodId;
    private readonly ServingSizeId _servingSizeId;
    private readonly float _quantity;

    public IngredientTests()
    {
        _userId = UserId.NewId();
        _id = IngredientId.NewId();
        _foodId = FoodId.NewId();
        _servingSizeId = ServingSizeId.NewId();
        _quantity = 100f;
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateIngredient()
    {
        // Act
        var result = Ingredient.Create(_userId, _id, _foodId, _servingSizeId, _quantity);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.FoodId.Should().Be(_foodId);
        result.Value.ServingSizeId.Should().Be(_servingSizeId);
        result.Value.Quantity.Should().Be(_quantity);
        result.Value.CreatedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.Value.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        // Arrange
        var emptyId = IngredientId.Empty;

        // Act
        var result = Ingredient.Create(_userId, emptyId, _foodId, _servingSizeId, _quantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Ingredient)}.{nameof(Ingredient.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithNullFoodId_ShouldFail()
    {
        // Arrange
        FoodId? nullFoodId = null;

        // Act
        var result = Ingredient.Create(_userId, _id, nullFoodId!, _servingSizeId, _quantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Ingredient)}.{nameof(Ingredient.FoodId).ToCapitalCase()}.Null");
    }

    [Fact]
    public void Create_WithNullServingSizeId_ShouldFail()
    {
        // Arrange
        ServingSizeId? nullServingSizeId = null;

        // Act
        var result = Ingredient.Create(_userId, _id, _foodId, nullServingSizeId!, _quantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Ingredient)}.{nameof(Ingredient.ServingSizeId).ToCapitalCase()}.Null");
    }

    [Fact]
    public void Create_WithNegativeQuantity_ShouldFail()
    {
        // Arrange
        var negativeQuantity = -100f;

        // Act
        var result = Ingredient.Create(_userId, _id, _foodId, _servingSizeId, negativeQuantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Ingredient)}.{nameof(Ingredient.Quantity).ToCapitalCase()}.NotPositive");
    }

    [Fact]
    public void Clone_WithValidId_ShouldCreateClone()
    {
        // Arrange
        var ingredient = Ingredient.Create(_userId, _id, _foodId, _servingSizeId, _quantity).Value;
        var newId = IngredientId.NewId();

        // Act
        var result = ingredient.Clone(newId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(newId);
        result.Value.FoodId.Should().Be(_foodId);
        result.Value.ServingSizeId.Should().Be(_servingSizeId);
        result.Value.Quantity.Should().Be(_quantity);
        result.Value.CreatedOnUtc.Should().Be(ingredient.CreatedOnUtc);
        result.Value.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Clone_WithEmptyId_ShouldFail()
    {
        // Arrange
        var ingredient = Ingredient.Create(_userId, _id, _foodId, _servingSizeId, _quantity).Value;
        var emptyId = IngredientId.Empty;

        // Act
        var result = ingredient.Clone(emptyId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Ingredient)}.{nameof(Ingredient.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void UpdateServingSize_WithValidId_ShouldUpdate()
    {
        // Arrange
        var ingredient = Ingredient.Create(_userId, _id, _foodId, _servingSizeId, _quantity).Value;
        var newServingSizeId = ServingSizeId.NewId();

        // Act
        var result = ingredient.UpdateServingSize(newServingSizeId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ingredient.ServingSizeId.Should().Be(newServingSizeId);
    }

    [Fact]
    public void UpdateServingSize_WithEmptyId_ShouldFail()
    {
        // Arrange
        var ingredient = Ingredient.Create(_userId, _id, _foodId, _servingSizeId, _quantity).Value;
        ServingSizeId? nullServingSizeId = null;

        // Act
        var result = ingredient.UpdateServingSize(nullServingSizeId!);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Ingredient)}.{nameof(Ingredient.ServingSizeId).ToCapitalCase()}.Null");
        ingredient.ServingSizeId.Should().Be(_servingSizeId);
        ingredient.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void UpdateQuantity_WithValidValue_ShouldUpdate()
    {
        // Arrange
        var ingredient = Ingredient.Create(_userId, _id, _foodId, _servingSizeId, _quantity).Value;
        var newQuantity = 200f;

        // Act
        var result = ingredient.UpdateQuantity(newQuantity);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ingredient.Quantity.Should().Be(newQuantity);
    }

    [Fact]
    public void UpdateQuantity_WithNegativeValue_ShouldFail()
    {
        // Arrange
        var ingredient = Ingredient.Create(_userId, _id, _foodId, _servingSizeId, _quantity).Value;
        var negativeQuantity = -100f;

        // Act
        var result = ingredient.UpdateQuantity(negativeQuantity);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Ingredient)}.{nameof(Ingredient.Quantity).ToCapitalCase()}.NotPositive");
        ingredient.Quantity.Should().Be(_quantity);
        ingredient.ModifiedOnUtc.Should().BeNull();
    }
}
