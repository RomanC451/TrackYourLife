using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UndoDeleteRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.UndoDeleteRecipe;

public class UndoDeleteRecipeCommandValidatorTests
{
    private readonly UndoDeleteRecipeCommandValidator _validator;

    public UndoDeleteRecipeCommandValidatorTests()
    {
        _validator = new UndoDeleteRecipeCommandValidator();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UndoDeleteRecipeCommand(RecipeId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var command = new UndoDeleteRecipeCommand(RecipeId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Id);
    }
}
