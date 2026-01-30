using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetCaloriesBurnedHistory;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetCaloriesBurnedHistory;

public class GetCaloriesBurnedHistoryQueryValidatorTests
{
    private readonly GetCaloriesBurnedHistoryQueryValidator _validator;

    public GetCaloriesBurnedHistoryQueryValidatorTests()
    {
        _validator = new GetCaloriesBurnedHistoryQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetCaloriesBurnedHistoryQuery();
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenEndDateBeforeStartDate_ShouldHaveValidationError()
    {
        var query = new GetCaloriesBurnedHistoryQuery(
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(-7)
        );
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.EndDate);
    }
}
