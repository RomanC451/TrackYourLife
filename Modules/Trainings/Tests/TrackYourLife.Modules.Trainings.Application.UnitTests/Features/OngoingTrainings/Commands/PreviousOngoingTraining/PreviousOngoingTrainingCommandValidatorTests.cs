using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.PreviousOngoingTraining;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.PreviousOngoingTraining;

public class PreviousOngoingTrainingCommandValidatorTests
{
    private readonly PreviousOngoingTrainingCommandValidator _validator;

    public PreviousOngoingTrainingCommandValidatorTests()
    {
        _validator = new PreviousOngoingTrainingCommandValidator();
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new PreviousOngoingTrainingCommand(OngoingTrainingId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OngoingTrainingId);
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new PreviousOngoingTrainingCommand(OngoingTrainingId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

