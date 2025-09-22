using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.DeleteOngoingTraining;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.DeleteOngoingTraining;

public class DeleteOngoingTrainingCommandValidatorTests
{
    private readonly DeleteOngoingTrainingCommandValidator _validator;

    public DeleteOngoingTrainingCommandValidatorTests()
    {
        _validator = new DeleteOngoingTrainingCommandValidator();
    }

    [Fact]
    public void Validate_WhenTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteOngoingTrainingCommand(TrainingId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TrainingId);
    }

    [Fact]
    public void Validate_WhenTrainingIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteOngoingTrainingCommand(TrainingId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

