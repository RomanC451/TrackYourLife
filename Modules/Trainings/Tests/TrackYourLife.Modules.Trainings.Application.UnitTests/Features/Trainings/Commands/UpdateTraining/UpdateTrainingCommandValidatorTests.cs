using System.Threading.Tasks;
using FluentValidation.TestHelper;
using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Features.Trainings.Commands.UpdateTraining;

public class UpdateTrainingCommandValidatorTests
{
    private readonly UpdateTrainingCommandValidator _validator;
    private readonly IExercisesQuery _exercisesQuery;

    public UpdateTrainingCommandValidatorTests()
    {
        _exercisesQuery = Substitute.For<IExercisesQuery>();
        _validator = new UpdateTrainingCommandValidator(_exercisesQuery);
    }

    [Fact]
    public async Task Validate_WhenTrainingIdIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTrainingCommand(
            TrainingId.Empty,
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            60,
            90,
            null,
            new List<ExerciseId> { ExerciseId.NewId() }
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TrainingId);
    }

    [Fact]
    public async Task Validate_WhenNameIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTrainingCommand(
            TrainingId.NewId(),
            "",
            new List<string> { "Chest" },
            Difficulty.Easy,
            60,
            90,
            null,
            new List<ExerciseId> { ExerciseId.NewId() }
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
        var command = new UpdateTrainingCommand(
            TrainingId.NewId(),
            new string('A', 101), // Too long name
            new List<string> { "Chest" },
            Difficulty.Easy,
            60,
            90,
            null,
            new List<ExerciseId> { ExerciseId.NewId() }
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
        var command = new UpdateTrainingCommand(
            TrainingId.NewId(),
            "Test Training",
            new List<string>(),
            Difficulty.Easy,
            60,
            90,
            null,
            new List<ExerciseId> { ExerciseId.NewId() }
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.MuscleGroups);
    }

    [Fact]
    public async Task Validate_WhenExerciseIdsIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTrainingCommand(
            TrainingId.NewId(),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            60,
            90,
            null,
            new List<ExerciseId>()
        );

        // Act
        var result = await _validator.TestValidateAsync(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ExerciseIds);
    }

    [Fact]
    public async Task Validate_WhenDurationIsNegative_ShouldHaveValidationError()
    {
        // Arrange
        var command = new UpdateTrainingCommand(
            TrainingId.NewId(),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            -1, // Negative duration
            90,
            null,
            new List<ExerciseId> { ExerciseId.NewId() }
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
        var command = new UpdateTrainingCommand(
            TrainingId.NewId(),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            60,
            -1, // Negative rest seconds
            null,
            new List<ExerciseId> { ExerciseId.NewId() }
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
        var command = new UpdateTrainingCommand(
            TrainingId.NewId(),
            "Test Training",
            new List<string> { "Chest", "Triceps" },
            Difficulty.Easy,
            60,
            90,
            "Test description",
            new List<ExerciseId> { exerciseId1, exerciseId2 }
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
