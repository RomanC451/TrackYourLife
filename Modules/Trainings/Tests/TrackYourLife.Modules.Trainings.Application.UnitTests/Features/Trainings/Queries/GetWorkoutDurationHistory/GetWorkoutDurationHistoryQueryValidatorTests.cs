using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutDurationHistory;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetWorkoutDurationHistory;

public class GetWorkoutDurationHistoryQueryValidatorTests
{
    private readonly GetWorkoutDurationHistoryQueryValidator _validator;

    public GetWorkoutDurationHistoryQueryValidatorTests()
    {
        _validator = new GetWorkoutDurationHistoryQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetWorkoutDurationHistoryQuery();
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEndDateBeforeStartDate_ShouldHaveValidationError()
    {
        var query = new GetWorkoutDurationHistoryQuery(
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(-7)
        );
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
