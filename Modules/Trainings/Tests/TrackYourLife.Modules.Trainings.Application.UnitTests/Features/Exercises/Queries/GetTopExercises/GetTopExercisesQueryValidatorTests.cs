using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetTopExercises;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Queries.GetTopExercises;

public class GetTopExercisesQueryValidatorTests
{
    private readonly GetTopExercisesQueryValidator _validator;

    public GetTopExercisesQueryValidatorTests()
    {
        _validator = new GetTopExercisesQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetTopExercisesQuery(Page: 1, PageSize: 10);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenPageZero_ShouldHaveValidationError()
    {
        var query = new GetTopExercisesQuery(Page: 0, PageSize: 10);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WhenPageSizeZero_ShouldHaveValidationError()
    {
        var query = new GetTopExercisesQuery(Page: 1, PageSize: 0);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WhenStartDateAfterEndDate_ShouldHaveValidationError()
    {
        var query = new GetTopExercisesQuery(
            Page: 1,
            PageSize: 10,
            StartDate: DateTime.UtcNow,
            EndDate: DateTime.UtcNow.AddDays(-7)
        );
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }
}
