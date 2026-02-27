using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetDifficultyDistribution;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetDifficultyDistribution;

public class GetDifficultyDistributionQueryValidatorTests
{
    private readonly GetDifficultyDistributionQueryValidator _validator;

    public GetDifficultyDistributionQueryValidatorTests()
    {
        _validator = new GetDifficultyDistributionQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetDifficultyDistributionQuery(StartDate: null, EndDate: null);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateAfterEndDate_ShouldHaveValidationError()
    {
        var query = new GetDifficultyDistributionQuery(
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7))
        );
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }
}
