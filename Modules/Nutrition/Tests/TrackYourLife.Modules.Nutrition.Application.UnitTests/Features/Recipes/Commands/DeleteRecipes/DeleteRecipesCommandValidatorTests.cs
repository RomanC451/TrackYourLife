using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.DeleteRecipes;

public class DeleteRecipesCommandValidatorTests
{
    private readonly DeleteRecipesCommandValidator _validator;

    public DeleteRecipesCommandValidatorTests()
    {
        _validator = new DeleteRecipesCommandValidator();
    }

    [Fact]
    public void Validate_WhenIdsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteRecipesCommand([RecipeId.NewId(), RecipeId.NewId()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdsAreEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteRecipesCommand([]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids).WithErrorMessage("Ids must not be empty");
    }

    [Fact]
    public void Validate_WhenIdsContainEmptyId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteRecipesCommand([RecipeId.Empty, RecipeId.NewId()]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids);
    }

    [Fact]
    public void Validate_WhenIdsContainDuplicates_ShouldHaveValidationError()
    {
        // Arrange
        var id = RecipeId.NewId();
        var command = new DeleteRecipesCommand([id, id]);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Ids).WithErrorMessage("Ids must be unique");
    }
}
