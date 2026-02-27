using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.JumpToExercise;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.JumpToExercise;

public class JumpToExerciseCommandValidatorTests
{
    private readonly JumpToExerciseCommandValidator _validator;

    public JumpToExerciseCommandValidatorTests()
    {
        _validator = new JumpToExerciseCommandValidator();
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new JumpToExerciseCommand(OngoingTrainingId.Empty, 0);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OngoingTrainingId);
    }

    [Fact]
    public void Validate_WhenExerciseIndexIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = new JumpToExerciseCommand(OngoingTrainingId.NewId(), -1);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExerciseIndex);
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdAndExerciseIndexAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new JumpToExerciseCommand(OngoingTrainingId.NewId(), 0);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
