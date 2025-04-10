using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipesByUserId;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Recipes.Queries.GetRecipesByUserId;

public class GetRecipesByUserIdQueryValidatorTests
{
    private readonly GetRecipesByUserIdQueryValidator _validator;

    public GetRecipesByUserIdQueryValidatorTests()
    {
        _validator = new GetRecipesByUserIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetRecipesByUserIdQuery();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
