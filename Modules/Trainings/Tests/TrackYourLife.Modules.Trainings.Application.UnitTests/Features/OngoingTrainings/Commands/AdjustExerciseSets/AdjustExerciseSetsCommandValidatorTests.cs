using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.AdjustExerciseSets;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.AdjustExerciseSets;

public class AdjustExerciseSetsCommandValidatorTests
{
    private readonly AdjustExerciseSetsCommandValidator _validator;

    public AdjustExerciseSetsCommandValidatorTests()
    {
        _validator = new AdjustExerciseSetsCommandValidator();
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AdjustExerciseSetsCommand(
            OngoingTrainingId.Empty,
            ExerciseId.NewId(),
            [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OngoingTrainingId);
    }

    [Fact]
    public void Validate_WhenExerciseIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AdjustExerciseSetsCommand(
            OngoingTrainingId.NewId(),
            ExerciseId.Empty,
            [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExerciseId);
    }

    [Fact]
    public void Validate_WhenExerciseSetChangesIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AdjustExerciseSetsCommand(
            OngoingTrainingId.NewId(),
            ExerciseId.NewId(),
            []
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewExerciseSets);
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new AdjustExerciseSetsCommand(
            OngoingTrainingId.NewId(),
            ExerciseId.NewId(),
            [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

