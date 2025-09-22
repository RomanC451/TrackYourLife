using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.FinishOngoingTraining;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.FinishOngoingTraining;

public class FinishOngoingTrainingCommandValidatorTests
{
    private readonly FinishOngoingTrainingCommandValidator _validator;

    public FinishOngoingTrainingCommandValidatorTests()
    {
        _validator = new FinishOngoingTrainingCommandValidator();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new FinishOngoingTrainingCommand(OngoingTrainingId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new FinishOngoingTrainingCommand(OngoingTrainingId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

