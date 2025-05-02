using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionGoals;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Queries.GetNutritionGoals;

public class GetNutritionGoalsQueryValidatorTests
{
    private readonly GetNutritionGoalsQueryValidator _validator;

    public GetNutritionGoalsQueryValidatorTests()
    {
        _validator = new GetNutritionGoalsQueryValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var query = new GetNutritionGoalsQuery(Date: DateOnly.FromDateTime(DateTime.UtcNow));

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithDefaultDate_ShouldBeInvalid()
    {
        // Arrange
        var query = new GetNutritionGoalsQuery(Date: default);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }
}
