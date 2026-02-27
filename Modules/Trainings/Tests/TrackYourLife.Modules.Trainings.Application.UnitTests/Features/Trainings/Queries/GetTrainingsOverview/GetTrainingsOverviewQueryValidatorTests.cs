using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsOverview;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetTrainingsOverview;

public class GetTrainingsOverviewQueryValidatorTests
{
    private readonly GetTrainingsOverviewQueryValidator _validator;

    public GetTrainingsOverviewQueryValidatorTests()
    {
        _validator = new GetTrainingsOverviewQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetTrainingsOverviewQuery(StartDate: null, EndDate: null);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateBeforeEndDate_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var query = new GetTrainingsOverviewQuery(StartDate: startDate, EndDate: endDate);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateAfterEndDate_ShouldHaveValidationError()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var query = new GetTrainingsOverviewQuery(StartDate: startDate, EndDate: endDate);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
        result
            .Errors.Where(e => e.PropertyName == nameof(query.StartDate))
            .Should()
            .Contain(e => e.ErrorMessage == "Start date must be less than or equal to end date");
    }
}
