using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;
using Xunit;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;

public class GetDailyNutritionOverviewsByDateRangeQueryValidatorTests
{
    private readonly GetDailyNutritionOverviewsByDateRangeQueryValidator _validator;

    public GetDailyNutritionOverviewsByDateRangeQueryValidatorTests()
    {
        _validator = new GetDailyNutritionOverviewsByDateRangeQueryValidator();
    }

    [Fact]
    public void Validate_WhenStartDateIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            default,
            DateOnly.FromDateTime(DateTime.Today)
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Validate_WhenEndDateIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            DateOnly.FromDateTime(DateTime.Today),
            default
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Validate_WhenEndDateIsBeforeStartDate_ShouldHaveValidationError()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            startDate.AddDays(-1)
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Validate_WhenDateRangeExceeds365Days_ShouldHaveValidationError()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            startDate.AddDays(366)
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Validate_WhenDatesAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(startDate, startDate.AddDays(7));

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartAndEndDateAreSame_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(date, date);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenDateRangeIs365Days_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            startDate.AddDays(365)
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
