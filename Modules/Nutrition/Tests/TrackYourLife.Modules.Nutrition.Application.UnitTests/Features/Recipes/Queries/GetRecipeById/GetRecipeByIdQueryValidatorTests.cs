using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipeById;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Queries.GetRecipeById;

public class GetRecipeByIdQueryValidatorTests
{
    private readonly GetRecipeByIdQueryValidator _validator;

    public GetRecipeByIdQueryValidatorTests()
    {
        _validator = new GetRecipeByIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetRecipeByIdQuery(RecipeId.NewId());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetRecipeByIdQuery(RecipeId.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
