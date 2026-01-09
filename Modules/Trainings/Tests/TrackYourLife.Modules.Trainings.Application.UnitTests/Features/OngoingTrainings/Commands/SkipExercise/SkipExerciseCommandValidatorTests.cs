using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.SkipExercise;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.SkipExercise;

public class SkipExerciseCommandValidatorTests
{
    private readonly SkipExerciseCommandValidator _validator;

    public SkipExerciseCommandValidatorTests()
    {
        _validator = new SkipExerciseCommandValidator();
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new SkipExerciseCommand(OngoingTrainingId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OngoingTrainingId);
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new SkipExerciseCommand(OngoingTrainingId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
