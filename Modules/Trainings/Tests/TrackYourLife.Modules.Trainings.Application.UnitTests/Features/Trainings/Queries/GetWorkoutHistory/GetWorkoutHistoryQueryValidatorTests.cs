using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutHistory;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetWorkoutHistory;

public class GetWorkoutHistoryQueryValidatorTests
{
    private readonly GetWorkoutHistoryQueryValidator _validator;

    public GetWorkoutHistoryQueryValidatorTests()
    {
        _validator = new GetWorkoutHistoryQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetWorkoutHistoryQuery(StartDate: null, EndDate: null);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateAndEndDateProvidedAndValid_ShouldNotHaveValidationErrors()
    {
        var startDate = DateTime.UtcNow.AddDays(-7);
        var endDate = DateTime.UtcNow;
        var query = new GetWorkoutHistoryQuery(StartDate: startDate, EndDate: endDate);

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateAfterEndDate_ShouldHaveValidationError()
    {
        var query = new GetWorkoutHistoryQuery(
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(-7)
        );

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }
}
