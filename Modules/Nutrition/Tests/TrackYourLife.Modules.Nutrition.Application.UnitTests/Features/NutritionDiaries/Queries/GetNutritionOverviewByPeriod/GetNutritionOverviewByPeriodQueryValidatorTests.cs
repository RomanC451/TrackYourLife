using FluentValidation.TestHelper;
using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;

public class GetNutritionOverviewByPeriodQueryValidatorTests
{
    private readonly GetNutritionOverviewByPeriodQueryValidator _validator;

    public GetNutritionOverviewByPeriodQueryValidatorTests()
    {
        _validator = new GetNutritionOverviewByPeriodQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetNutritionOverviewByPeriodQuery(
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateIsDefault_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetNutritionOverviewByPeriodQuery(
            StartDate: default,
            EndDate: DateOnly.FromDateTime(DateTime.Now)
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Validate_WhenEndDateIsDefault_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetNutritionOverviewByPeriodQuery(
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: default
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
        var query = new GetNutritionOverviewByPeriodQuery(
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }

    [Fact]
    public void Validate_WhenPeriodIsMoreThanOneYear_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetNutritionOverviewByPeriodQuery(
            StartDate: DateOnly.FromDateTime(DateTime.Now),
            EndDate: DateOnly.FromDateTime(DateTime.Now.AddDays(366))
        );

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
