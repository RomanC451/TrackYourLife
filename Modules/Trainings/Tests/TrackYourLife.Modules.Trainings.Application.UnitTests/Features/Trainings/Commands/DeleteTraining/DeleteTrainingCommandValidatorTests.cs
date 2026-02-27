using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.DeleteTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Commands.DeleteTraining;

public class DeleteTrainingCommandValidatorTests
{
    private readonly DeleteTrainingCommandValidator _validator;

    public DeleteTrainingCommandValidatorTests()
    {
        _validator = new DeleteTrainingCommandValidator();
    }

    [Fact]
    public void Validate_WhenTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteTrainingCommand(TrainingId.Empty, false);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TrainingId);
    }

    [Fact]
    public void Validate_WhenTrainingIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteTrainingCommand(TrainingId.NewId(), true);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

