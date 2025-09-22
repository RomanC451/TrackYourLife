using FluentValidation.TestHelper;
using System.Threading.Tasks;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.CreateExercise;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Exercises.Commands.CreateExercise;

public class CreateExerciseCommandValidatorTests
{
    private readonly CreateExerciseCommandValidator _validator;
    private readonly IExercisesQuery _exercisesQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;

    public CreateExerciseCommandValidatorTests()
    {
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _validator = new CreateExerciseCommandValidator(_exercisesQuery, _userIdentifierProvider);
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateExerciseCommand(
            "",
            ["Chest"],
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            [new ExerciseSet(Guid.NewGuid(), "Set 1", 10, 60, 0)]
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
        var command = new CreateExerciseCommand(
            new string('A', 101), // Too long name
            ["Chest"],
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            [new ExerciseSet(Guid.NewGuid(), "Set 1", 10, 60, 0)]
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
        var command = new CreateExerciseCommand(
            "Test Exercise",
            [],
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            [new ExerciseSet(Guid.NewGuid(), "Set 1", 10, 60, 0)]
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MuscleGroups);
    }

    [Fact]
    public async Task Validate_WhenSetsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateExerciseCommand(
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
        result.ShouldHaveValidationErrorFor(x => x.Sets);
    }

    [Fact]
    public async Task Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var command = new CreateExerciseCommand(
            "Test Exercise",
            ["Chest", "Triceps"],
            Difficulty.Easy,
            "picture-url",
            "video-url",
            "Description",
            "Barbell",
            [new ExerciseSet(Guid.NewGuid(), "Set 1", 10, 60, 0)]
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
