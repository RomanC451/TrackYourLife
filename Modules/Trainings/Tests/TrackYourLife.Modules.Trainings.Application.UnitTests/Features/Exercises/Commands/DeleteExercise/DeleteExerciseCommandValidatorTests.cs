using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.DeleteExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Commands.DeleteExercise;

public class DeleteExerciseCommandValidatorTests
{
    private readonly DeleteExerciseCommandValidator _validator;

    public DeleteExerciseCommandValidatorTests()
    {
        _validator = new DeleteExerciseCommandValidator();
    }

    [Fact]
    public void Validate_WhenExerciseIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteExerciseCommand(ExerciseId.Empty, false);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExerciseId);
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteExerciseCommand(ExerciseId.NewId(), true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

