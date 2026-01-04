using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;

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
            DateOnly.FromDateTime(DateTime.Today),
            OverviewType.Daily,
            AggregationMode.Sum
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
            default,
            OverviewType.Daily,
            AggregationMode.Sum
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
            startDate.AddDays(-1),
            OverviewType.Daily,
            AggregationMode.Sum
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Validate_WhenOverviewTypeIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            startDate.AddDays(7),
            (OverviewType)999, // Invalid enum value
            AggregationMode.Sum
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OverviewType);
    }

    [Fact]
    public void Validate_WhenAggregationModeIsInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            startDate.AddDays(7),
            OverviewType.Daily,
            (AggregationMode)999 // Invalid enum value
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AggregationMode);
    }

    [Fact]
    public void Validate_WhenDatesAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            startDate.AddDays(7),
            OverviewType.Daily,
            AggregationMode.Sum
        );

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
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            date,
            date,
            OverviewType.Weekly,
            AggregationMode.Average
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(OverviewType.Daily)]
    [InlineData(OverviewType.Weekly)]
    [InlineData(OverviewType.Monthly)]
    public void Validate_WithValidOverviewType_ShouldNotHaveValidationErrors(
        OverviewType overviewType
    )
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            startDate.AddDays(7),
            overviewType,
            AggregationMode.Sum
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(AggregationMode.Average)]
    [InlineData(AggregationMode.Sum)]
    public void Validate_WithValidAggregationMode_ShouldNotHaveValidationErrors(
        AggregationMode aggregationMode
    )
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var query = new GetDailyNutritionOverviewsByDateRangeQuery(
            startDate,
            startDate.AddDays(7),
            OverviewType.Daily,
            aggregationMode
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
