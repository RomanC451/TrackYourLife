using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.CreateOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.CreateOngoingTraining;

public class CreateOngoingTrainingCommandValidatorTests
{
    private readonly CreateOngoingTrainingCommandValidator _validator;

    public CreateOngoingTrainingCommandValidatorTests()
    {
        _validator = new CreateOngoingTrainingCommandValidator();
    }

    [Fact]
    public void Validate_WhenTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateOngoingTrainingCommand(TrainingId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TrainingId);
    }

    [Fact]
    public void Validate_WhenTrainingIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateOngoingTrainingCommand(TrainingId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

