using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExercisePerformance;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.ExercisesHistories.Queries.GetExercisePerformance;

public class GetExercisePerformanceQueryValidatorTests
{
    private readonly GetExercisePerformanceQueryValidator _validator;

    public GetExercisePerformanceQueryValidatorTests()
    {
        _validator = new GetExercisePerformanceQueryValidator();
    }

    [Fact]
    public void Validate_WhenQueryIsValid_ShouldNotHaveValidationErrors()
    {
        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenStartDateAfterEndDate_ShouldHaveValidationError()
    {
        var query = new GetExercisePerformanceQuery(
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 10
        );

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.StartDate);
    }

    [Fact]
    public void Validate_WhenPageZero_ShouldHaveValidationError()
    {
        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 0,
            PageSize: 10
        );

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Fact]
    public void Validate_WhenPageSizeZero_ShouldHaveValidationError()
    {
        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 0
        );

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WhenPageSizeOver100_ShouldHaveValidationError()
    {
        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: null,
            PerformanceCalculationMethod.Sequential,
            Page: 1,
            PageSize: 101
        );

        var result = _validator.TestValidate(query);

        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Fact]
    public void Validate_WhenExerciseIdProvided_ShouldNotHaveValidationError()
    {
        var query = new GetExercisePerformanceQuery(
            StartDate: null,
            EndDate: null,
            ExerciseId: ExerciseId.NewId(),
            PerformanceCalculationMethod.FirstVsLast,
            Page: 1,
            PageSize: 10
        );

        var result = _validator.TestValidate(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
