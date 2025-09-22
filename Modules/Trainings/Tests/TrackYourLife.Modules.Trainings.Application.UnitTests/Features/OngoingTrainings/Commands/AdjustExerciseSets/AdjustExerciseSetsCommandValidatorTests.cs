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
            [ExerciseSetChangeFaker.Generate()]
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
            [ExerciseSetChangeFaker.Generate()]
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
        result.ShouldHaveValidationErrorFor(x => x.ExerciseSetChanges);
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new AdjustExerciseSetsCommand(
            OngoingTrainingId.NewId(),
            ExerciseId.NewId(),
            [ExerciseSetChangeFaker.Generate()]
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

