using FluentValidation.TestHelper;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetGoalByType;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Queries.GetGoalByType;

public class GetGoalByTypeQueryValidatorTests
{
    private readonly GetGoalByTypeQueryValidator _validator;

    public GetGoalByTypeQueryValidatorTests()
    {
        _validator = new GetGoalByTypeQueryValidator();
    }

    [Fact]
    public void Validate_WithValidInput_ShouldBeValid()
    {
        // Arrange
        var query = new GetGoalByTypeQuery(
            Type: GoalType.Calories,
            Date: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithInvalidGoalType_ShouldBeInvalid()
    {
        // Arrange
        var query = new GetGoalByTypeQuery(
            Type: (GoalType)999, // Invalid enum value
            Date: DateOnly.FromDateTime(DateTime.UtcNow)
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Validate_WithDefaultDate_ShouldBeInvalid()
    {
        // Arrange
        var query = new GetGoalByTypeQuery(Type: GoalType.Calories, Date: default);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }
}
