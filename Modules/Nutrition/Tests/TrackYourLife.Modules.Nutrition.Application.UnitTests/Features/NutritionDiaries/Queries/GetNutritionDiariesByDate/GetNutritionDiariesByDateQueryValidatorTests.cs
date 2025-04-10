using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;

public class GetNutritionDiariesByDateQueryValidatorTests
{
    private readonly GetNutritionDiariesByDateQueryValidator _validator;

    public GetNutritionDiariesByDateQueryValidatorTests()
    {
        _validator = new GetNutritionDiariesByDateQueryValidator();
    }

    [Fact]
    public void Validate_WhenDayIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetNutritionDiariesByDateQuery(default);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Day);
    }

    [Fact]
    public void Validate_WhenDayIsValid_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetNutritionDiariesByDateQuery(DateOnly.FromDateTime(DateTime.Today));

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Day);
    }
}
