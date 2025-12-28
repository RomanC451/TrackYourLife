using System.Threading.Tasks;
using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Commands.UpdateExercise;

public class UpdateExerciseCommandValidatorTests
{
    private readonly UpdateExerciseCommandValidator _validator;
    private readonly IExercisesQuery _exercisesQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public UpdateExerciseCommandValidatorTests()
    {
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _validator = new UpdateExerciseCommandValidator(_exercisesQuery, _userIdentifierProvider);
    }

    [Fact]
    public async Task Validate_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateExerciseCommand(
            ExerciseId.Empty,
            "Test Exercise",
            ["Chest"],
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 60, "kg").Value]
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateExerciseCommand(
            ExerciseId.NewId(),
            "",
            ["Chest"],
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 60, "kg").Value]
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_WhenNameIsTooLong_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateExerciseCommand(
            ExerciseId.NewId(),
            new string('A', 101), // Too long name
            ["Chest"],
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 60, "kg").Value]
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_WhenMuscleGroupsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateExerciseCommand(
            ExerciseId.NewId(),
            "Test Exercise",
            [],
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 60, "kg").Value]
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MuscleGroups);
    }

    [Fact]
    public async Task Validate_WhenExerciseSetsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateExerciseCommand(
            ExerciseId.NewId(),
            "Test Exercise",
            ["Chest"],
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            []
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExerciseSets);
    }

    [Fact]
    public async Task Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new UpdateExerciseCommand(
            ExerciseId.NewId(),
            "Test Exercise",
            ["Chest", "Triceps"],
            Difficulty.Easy,
            "picture-url",
            "video-url",
            "Description",
            "Barbell",
            [ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 60, "kg").Value]
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
