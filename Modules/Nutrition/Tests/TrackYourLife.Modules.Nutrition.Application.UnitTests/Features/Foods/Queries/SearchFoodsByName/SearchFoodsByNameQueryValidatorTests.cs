using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Foods.Queries.SearchFoodsByName;

public class SearchFoodsByNameQueryValidatorTests
{
    private readonly SearchFoodsByNameQueryValidator _validator;

    public SearchFoodsByNameQueryValidatorTests()
    {
        _validator = new SearchFoodsByNameQueryValidator();
    }

    [Fact]
    public void Validate_WhenSearchParamIsNull_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery(null!, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchParam);
    }

    [Fact]
    public void Validate_WhenSearchParamIsEmpty_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery(string.Empty, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SearchParam);
    }

    [Fact]
    public void Validate_WhenPageIsLessThanOne_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("test", 0, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WhenPageSizeIsLessThanOne_ShouldHaveValidationError()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("test", 1, 0);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WhenAllParametersAreValid_ShouldBeValid()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("test", 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
