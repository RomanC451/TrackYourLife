using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;

public class GetExerciseHistoryByExerciseIdQueryValidatorTests
{
    private readonly GetExerciseHistoryByExerciseIdQueryValidator _validator;

    public GetExerciseHistoryByExerciseIdQueryValidatorTests()
    {
        _validator = new GetExerciseHistoryByExerciseIdQueryValidator();
    }

    [Fact]
    public void Validate_WhenExerciseIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetExerciseHistoryByExerciseIdQuery(ExerciseId.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExerciseId);
    }

    [Fact]
    public void Validate_WhenExerciseIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetExerciseHistoryByExerciseIdQuery(ExerciseId.NewId());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

