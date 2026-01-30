using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingTemplatesUsage;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetTrainingTemplatesUsage;

public class GetTrainingTemplatesUsageQueryValidatorTests
{
    private readonly GetTrainingTemplatesUsageQueryValidator _validator;

    public GetTrainingTemplatesUsageQueryValidatorTests()
    {
        _validator = new GetTrainingTemplatesUsageQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetTrainingTemplatesUsageQuery();

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateBeforeEndDate_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        var query = new GetTrainingTemplatesUsageQuery(StartDate: startDate, EndDate: endDate);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateAfterEndDate_ShouldHaveValidationError()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(-7);
        var query = new GetTrainingTemplatesUsageQuery(StartDate: startDate, EndDate: endDate);

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
