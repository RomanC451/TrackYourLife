using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetMuscleGroupDistribution;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Queries.GetMuscleGroupDistribution;

public class GetMuscleGroupDistributionQueryValidatorTests
{
    private readonly GetMuscleGroupDistributionQueryValidator _validator;

    public GetMuscleGroupDistributionQueryValidatorTests()
    {
        _validator = new GetMuscleGroupDistributionQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetMuscleGroupDistributionQuery(StartDate: null, EndDate: null);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateAfterEndDate_ShouldHaveValidationError()
    {
        var query = new GetMuscleGroupDistributionQuery(
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(-7)
        );
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }
}
