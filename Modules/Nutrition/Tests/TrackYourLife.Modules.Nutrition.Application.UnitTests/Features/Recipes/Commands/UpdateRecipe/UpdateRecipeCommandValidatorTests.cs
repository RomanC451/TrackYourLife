using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandValidatorTests
{
    private readonly UpdateRecipeCommandValidator _validator;

    public UpdateRecipeCommandValidatorTests()
    {
        _validator = new UpdateRecipeCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: "Test Recipe",
            Portions: 4
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenRecipeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.Empty,
            Name: "Test Recipe",
            Portions: 4
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecipeId);
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: string.Empty,
            Portions: 4
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WhenNameIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: new string('a', 101),
            Portions: 4
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenPortionsIsLessThanOrEqualToZero_ShouldHaveValidationError(int portions)
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: "Test Recipe",
            Portions: portions
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Portions);
    }
}
