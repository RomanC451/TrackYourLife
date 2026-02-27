using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutFrequency;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetWorkoutFrequency;

public class GetWorkoutFrequencyQueryValidatorTests
{
    private readonly GetWorkoutFrequencyQueryValidator _validator;

    public GetWorkoutFrequencyQueryValidatorTests()
    {
        _validator = new GetWorkoutFrequencyQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetWorkoutFrequencyQuery();
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEndDateBeforeStartDate_ShouldHaveValidationError()
    {
        var query = new GetWorkoutFrequencyQuery(
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7))
        );
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
