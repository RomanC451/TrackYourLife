using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidatorTests
{
    private readonly CreateRecipeCommandValidator _validator;

    public CreateRecipeCommandValidatorTests()
    {
        _validator = new CreateRecipeCommandValidator();
    }

    [Fact]
    public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateRecipeCommand("Test Recipe");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateRecipeCommand(string.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WhenNameIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateRecipeCommand(null!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
