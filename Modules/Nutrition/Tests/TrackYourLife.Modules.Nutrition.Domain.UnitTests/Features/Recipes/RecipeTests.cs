using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.UnitTests.Recipes;

public class RecipeTests
{
    private readonly RecipeId _id;
    private readonly UserId _userId;
    private readonly string _name;
    private readonly Ingredient _ingredient;
    private readonly Food _food;
    private readonly ServingSizeReadModel _servingSize;

    public RecipeTests()
    {
        _id = RecipeId.NewId();
        _userId = UserId.NewId();
        _name = "Test Recipe";

        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();

        _ingredient = Ingredient
            .Create(_userId, IngredientId.NewId(), foodId, servingSizeId, 100f)
            .Value;

        _food = Food.Create(
            foodId,
            "Food",
            "Test Brand",
            "US",
            "Test Food",
            new NutritionalContent(),
            [FoodServingSize.Create(foodId, servingSizeId, 1).Value]
        ).Value;

        _servingSize = new ServingSizeReadModel(servingSizeId, 1f, "g", 100f, null);
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateRecipe()
    {
        // Act
        var result = Recipe.Create(_id, _userId, _name, 100f, 1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_id);
        result.Value.UserId.Should().Be(_userId);
        result.Value.Name.Should().Be(_name);
        result.Value.Portions.Should().Be(1); // Default value
        result.Value.Ingredients.Should().BeEmpty();
        result.Value.NutritionalContents.Should().NotBeNull();
        result.Value.IsOld.Should().BeFalse();
        result.Value.CreatedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.Value.ModifiedOnUtc.Should().BeNull();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldFail()
    {
        // Act
        var result = Recipe.Create(RecipeId.Empty, _userId, _name, 100f, 1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Recipe)}.{nameof(Recipe.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldFail()
    {
        // Act
        var result = Recipe.Create(_id, UserId.Empty, _name, 100f, 1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Recipe)}.{nameof(Recipe.UserId).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Create_WithEmptyName_ShouldFail()
    {
        // Act
        var result = Recipe.Create(_id, _userId, string.Empty, 100f, 1);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Recipe)}.{nameof(Recipe.Name).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void Clone_WithValidId_ShouldCreateClone()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.AddIngredient(_ingredient, _food, _servingSize);
        var newId = RecipeId.NewId();

        // Act
        var result = recipe.Clone(newId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(newId);
        result.Value.UserId.Should().Be(_userId);
        result.Value.Name.Should().Be(_name);
        result.Value.Portions.Should().Be(recipe.Portions);
        result.Value.Ingredients.Should().HaveCount(1);
        result.Value.NutritionalContents.Should().NotBeNull();
        result.Value.IsOld.Should().BeFalse();
    }

    [Fact]
    public void Clone_WithEmptyId_ShouldFail()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;

        // Act
        var result = recipe.Clone(RecipeId.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Recipe)}.{nameof(Recipe.Id).ToCapitalCase()}.Empty");
    }

    [Fact]
    public void AddIngredient_WithValidData_ShouldAddIngredient()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;

        // Act
        var result = recipe.AddIngredient(_ingredient, _food, _servingSize);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.Ingredients.Should().ContainSingle();
        recipe.Ingredients[0].Should().Be(_ingredient);
    }

    [Fact]
    public void AddIngredient_WhenRecipeIsOld_ShouldFail()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.MarkAsOld();

        // Act
        var result = recipe.AddIngredient(_ingredient, _food, _servingSize);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.Old);
        recipe.Ingredients.Should().BeEmpty();
    }

    [Fact]
    public void AddIngredient_WithNullIngredient_ShouldFail()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;

        // Act
        var result = recipe.AddIngredient(null!, _food, _servingSize);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Recipe)}.{nameof(Ingredient).ToCapitalCase()}.Null");
        recipe.Ingredients.Should().BeEmpty();
    }

    [Fact]
    public void RemoveIngredient_WithValidData_ShouldRemoveIngredient()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.AddIngredient(_ingredient, _food, _servingSize);

        // Act
        var result = recipe.RemoveIngredient(_ingredient, _food, _servingSize);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.Ingredients.Should().BeEmpty();
    }

    [Fact]
    public void RemoveIngredient_WhenRecipeIsOld_ShouldFail()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.AddIngredient(_ingredient, _food, _servingSize);
        recipe.MarkAsOld();

        // Act
        var result = recipe.RemoveIngredient(_ingredient, _food, _servingSize);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(RecipeErrors.Old);
        recipe.Ingredients.Should().ContainSingle();
    }

    [Fact]
    public void UpdateIngredient_WithValidData_ShouldUpdateIngredient()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.AddIngredient(_ingredient, _food, _servingSize);

        var newServingSize = ServingSize.Create(ServingSizeId.NewId(), 2f, "g", 200f).Value;
        var updatedIngredient = Ingredient
            .Create(_userId, _ingredient.Id, _ingredient.FoodId, newServingSize.Id, 200f)
            .Value;

        // Act
        var result = recipe.UpdateIngredient(
            _food,
            updatedIngredient,
            ServingSize
                .Create(
                    _servingSize.Id,
                    _servingSize.NutritionMultiplier,
                    _servingSize.Unit,
                    _servingSize.Value
                )
                .Value,
            newServingSize,
            _ingredient.Quantity
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.Ingredients.Should().ContainSingle();
        recipe.Ingredients[0].Should().Be(updatedIngredient);
    }

    [Fact]
    public void UpdateIngredient_WithNonExistentIngredient_ShouldFail()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        var nonExistentIngredient = Ingredient
            .Create(_userId, IngredientId.NewId(), _food.Id, _servingSize.Id, 100f)
            .Value;

        // Act
        var result = recipe.UpdateIngredient(
            _food,
            nonExistentIngredient,
            ServingSize
                .Create(
                    _servingSize.Id,
                    _servingSize.NutritionMultiplier,
                    _servingSize.Unit,
                    _servingSize.Value
                )
                .Value,
            ServingSize
                .Create(
                    _servingSize.Id,
                    _servingSize.NutritionMultiplier,
                    _servingSize.Unit,
                    _servingSize.Value
                )
                .Value,
            100f
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be(IngredientErrors.NotFound(nonExistentIngredient.Id).Code);
    }

    [Fact]
    public void UpdatePortions_WithValidValue_ShouldUpdatePortions()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        var newPortions = 6;

        // Act
        var result = recipe.UpdatePortions(newPortions);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.Portions.Should().Be(newPortions);
    }

    [Fact]
    public void UpdatePortions_WithNegativeValue_ShouldFail()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        var negativePortions = -1;

        // Act
        var result = recipe.UpdatePortions(negativePortions);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Recipe)}.{nameof(Recipe.Portions).ToCapitalCase()}.NotPositive");
        recipe.Portions.Should().Be(1); // Default value
    }

    [Fact]
    public void UpdateName_WithValidValue_ShouldUpdateName()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        var newName = "New Recipe Name";

        // Act
        var result = recipe.UpdateName(newName);

        // Assert
        result.IsSuccess.Should().BeTrue();
        recipe.Name.Should().Be(newName);
    }

    [Fact]
    public void UpdateName_WithEmptyValue_ShouldFail()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;

        // Act
        var result = recipe.UpdateName(string.Empty);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .Be($"{nameof(Recipe)}.{nameof(Recipe.Name).ToCapitalCase()}.Empty");
        recipe.Name.Should().Be(_name);
    }

    [Fact]
    public void GetIngredientById_WithExistingId_ShouldReturnIngredient()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.AddIngredient(_ingredient, _food, _servingSize);

        // Act
        var result = recipe.GetIngredientById(_ingredient.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_ingredient);
    }

    [Fact]
    public void GetIngredientById_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.AddIngredient(_ingredient, _food, _servingSize);

        // Act
        var result = recipe.GetIngredientById(IngredientId.NewId());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetIngredientByFoodId_WithExistingId_ShouldReturnIngredient()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.AddIngredient(_ingredient, _food, _servingSize);

        // Act
        var result = recipe.GetIngredientByFoodId(_ingredient.FoodId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(_ingredient);
    }

    [Fact]
    public void GetIngredientByFoodId_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.AddIngredient(_ingredient, _food, _servingSize);

        // Act
        var result = recipe.GetIngredientByFoodId(FoodId.NewId());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void MarkAsOld_ShouldMarkRecipeAsOld()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;

        // Act
        recipe.MarkAsOld();

        // Assert
        recipe.IsOld.Should().BeTrue();
    }

    [Fact]
    public void RemoveOldMark_ShouldRemoveOldMark()
    {
        // Arrange
        var recipe = Recipe.Create(_id, _userId, _name, 100f, 1).Value;
        recipe.MarkAsOld();

        // Act
        recipe.RemoveOldMark();

        // Assert
        recipe.IsOld.Should().BeFalse();
    }
}
