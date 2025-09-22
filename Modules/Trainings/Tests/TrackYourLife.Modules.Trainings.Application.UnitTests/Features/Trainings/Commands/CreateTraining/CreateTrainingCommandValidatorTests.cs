using System.Threading.Tasks;
using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.CreateTraining;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Commands.CreateTraining;

public class CreateTrainingCommandValidatorTests
{
    private readonly CreateTrainingCommandValidator _validator;
    private readonly ITrainingsQuery _trainingsQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IExercisesQuery _exercisesQuery;

    public CreateTrainingCommandValidatorTests()
    {
        _trainingsQuery = Substitute.For<ITrainingsQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _validator = new CreateTrainingCommandValidator(
            _trainingsQuery,
            _userIdentifierProvider,
            _exercisesQuery
        );
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateTrainingCommand(
            "",
            ["Chest"],
            Difficulty.Easy,
            [ExerciseId.NewId()],
            60,
            90,
            null
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
        var command = new CreateTrainingCommand(
            new string('A', 101), // Too long name
            ["Chest"],
            Difficulty.Easy,
            [ExerciseId.NewId()],
            60,
            90,
            null
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
        var command = new CreateTrainingCommand(
            "Test Training",
            [],
            Difficulty.Easy,
            [ExerciseId.NewId()],
            60,
            90,
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MuscleGroups);
    }

    [Fact]
    public async Task Validate_WhenExercisesIdsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateTrainingCommand(
            "Test Training",
            ["Chest"],
            Difficulty.Easy,
            [],
            60,
            90,
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExercisesIds);
    }

    [Fact]
    public async Task Validate_WhenDurationIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateTrainingCommand(
            "Test Training",
            ["Chest"],
            Difficulty.Easy,
            [ExerciseId.NewId()],
            -1, // Negative duration
            90,
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Duration);
    }

    [Fact]
    public async Task Validate_WhenRestSecondsIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = new CreateTrainingCommand(
            "Test Training",
            ["Chest"],
            Difficulty.Easy,
            [ExerciseId.NewId()],
            60,
            -1, // Negative rest seconds
            null
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.RestSeconds);
    }

    [Fact]
    public async Task Validate_WhenAllFieldsAreValid_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var exerciseId1 = ExerciseId.NewId();
        var exerciseId2 = ExerciseId.NewId();
        var command = new CreateTrainingCommand(
            "Test Training",
            ["Chest", "Triceps"],
            Difficulty.Easy,
            [exerciseId1, exerciseId2],
            60,
            90,
            "Test description"
        );

        // Mock exercises to exist
        var exercises = new List<ExerciseReadModel>
        {
            ExerciseReadModelFaker.Generate(id: exerciseId1),
            ExerciseReadModelFaker.Generate(id: exerciseId2),
        };
        _exercisesQuery
            .GetEnumerableWithinIdsCollectionAsync(
                Arg.Is<List<ExerciseId>>(ids =>
                    ids.Contains(exerciseId1) && ids.Contains(exerciseId2)
                ),
                Arg.Any<CancellationToken>()
            )
            .Returns(exercises);

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
