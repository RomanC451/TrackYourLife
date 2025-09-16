using FluentValidation.TestHelper;
using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandValidatorTests
{
    private readonly UpdateRecipeCommandValidator _validator;

    public UpdateRecipeCommandValidatorTests()
    {
        _validator = new UpdateRecipeCommandValidator(
            Substitute.For<IRecipeQuery>(),
            Substitute.For<IUserIdentifierProvider>()
        );
    }

    [Fact]
    public async Task Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: "Test Recipe",
            Portions: 4,
            Weight: 100f
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_WhenRecipeIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.Empty,
            Name: "Test Recipe",
            Portions: 4,
            Weight: 100f
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RecipeId);
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: string.Empty,
            Portions: 4,
            Weight: 100f
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_WhenNameIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: new string('a', 101),
            Portions: 4,
            Weight: 100f
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validate_WhenPortionsIsLessThanOrEqualToZero_ShouldHaveValidationError(
        int portions
    )
    {
        // Arrange
        var command = new UpdateRecipeCommand(
            RecipeId: RecipeId.NewId(),
            Name: "Test Recipe",
            Portions: portions,
            Weight: 100f
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Portions);
    }
}
