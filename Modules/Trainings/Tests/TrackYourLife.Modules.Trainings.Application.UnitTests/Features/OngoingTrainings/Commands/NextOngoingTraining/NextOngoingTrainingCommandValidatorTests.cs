using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Commands.NextOngoingTraining;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.OngoingTrainings.Commands.NextOngoingTraining;

public class NextOngoingTrainingCommandValidatorTests
{
    private readonly NextOngoingTrainingCommandValidator _validator;

    public NextOngoingTrainingCommandValidatorTests()
    {
        _validator = new NextOngoingTrainingCommandValidator();
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new NextOngoingTrainingCommand(OngoingTrainingId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OngoingTrainingId);
    }

    [Fact]
    public void Validate_WhenOngoingTrainingIdIsValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new NextOngoingTrainingCommand(OngoingTrainingId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}

