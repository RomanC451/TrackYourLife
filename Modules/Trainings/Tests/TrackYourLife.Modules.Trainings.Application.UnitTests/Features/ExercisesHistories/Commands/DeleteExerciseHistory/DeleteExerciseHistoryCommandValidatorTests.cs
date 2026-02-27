using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Commands.DeleteExerciseHistory;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.ExercisesHistories.Commands.DeleteExerciseHistory;

public class DeleteExerciseHistoryCommandValidatorTests
{
    private readonly DeleteExerciseHistoryCommandValidator _validator;

    public DeleteExerciseHistoryCommandValidatorTests()
    {
        _validator = new DeleteExerciseHistoryCommandValidator();
    }

    [Fact]
    public void Validate_WhenExerciseHistoryIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new DeleteExerciseHistoryCommand(ExerciseHistoryId.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExerciseHistoryId);
    }

    [Fact]
    public void Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new DeleteExerciseHistoryCommand(ExerciseHistoryId.NewId());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
