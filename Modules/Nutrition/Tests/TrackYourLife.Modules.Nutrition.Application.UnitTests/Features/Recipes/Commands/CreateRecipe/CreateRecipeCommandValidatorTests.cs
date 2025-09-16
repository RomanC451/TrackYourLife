using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidatorTests
{
    private readonly CreateRecipeCommandValidator _validator;

    public CreateRecipeCommandValidatorTests()
    {
        _validator = new CreateRecipeCommandValidator(
            Substitute.For<IRecipeQuery>(),
            Substitute.For<IUserIdentifierProvider>()
        );
    }

    [Fact]
    public async Task Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateRecipeCommand("Test Recipe", 1, 100f);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateRecipeCommand(string.Empty, 1, 100f);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_WhenNameIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateRecipeCommand(null!, 1, 100f);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
