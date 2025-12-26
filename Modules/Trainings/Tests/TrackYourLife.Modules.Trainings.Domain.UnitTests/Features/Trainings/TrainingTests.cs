using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.Trainings;

public class TrainingTests
{
    private readonly TrainingId _validId = TrainingId.Create(Guid.NewGuid());
    private readonly UserId _validUserId = UserId.Create(Guid.NewGuid());
    private readonly string _validName = "Test Training";
    private readonly List<string> _validMuscleGroups = new() { "Chest", "Triceps" };
    private readonly Difficulty _validDifficulty = Difficulty.Medium;
    private readonly int _validDuration = 45;
    private readonly int _validRestSeconds = 90;
    private readonly string _validDescription = "Test training description";
    private readonly DateTime _validCreatedOnUtc = DateTime.UtcNow;

    private List<TrainingExercise> CreateValidTrainingExercises()
    {
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        var exercise = Exercise
            .Create(
                exerciseId,
                _validUserId,
                "Test Exercise",
                new List<string> { "Chest" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50.0f),
                    new WeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 8, 60.0f),
                },
                DateTime.UtcNow
            )
            .Value;

        var trainingExercise = TrainingExercise.Create(_validId, exercise, 0).Value;
        return new List<TrainingExercise> { trainingExercise };
    }

    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var trainingExercises = CreateValidTrainingExercises();

        // Act
        var result = Training.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            trainingExercises,
            _validCreatedOnUtc,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        var training = result.Value;
        training.Id.Should().Be(_validId);
        training.UserId.Should().Be(_validUserId);
        training.Name.Should().Be(_validName);
        training.MuscleGroups.Should().BeEquivalentTo(_validMuscleGroups);
        training.Difficulty.Should().Be(_validDifficulty);
        training.Duration.Should().Be(_validDuration);
        training.RestSeconds.Should().Be(_validRestSeconds);
        training.Description.Should().Be(_validDescription);
        training.TrainingExercises.Should().BeEquivalentTo(trainingExercises);
        training.CreatedOnUtc.Should().Be(_validCreatedOnUtc);
    }

    [Fact]
    public void Create_WithEmptyId_ShouldReturnFailure()
    {
        // Arrange
        var emptyId = TrainingId.Empty;
        var trainingExercises = CreateValidTrainingExercises();

        // Act
        var result = Training.Create(
            emptyId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            trainingExercises,
            _validCreatedOnUtc,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldReturnFailure()
    {
        // Arrange
        var emptyUserId = UserId.Empty;
        var trainingExercises = CreateValidTrainingExercises();

        // Act
        var result = Training.Create(
            _validId,
            emptyUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            trainingExercises,
            _validCreatedOnUtc,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldReturnFailure(string? name)
    {
        // Arrange
        var trainingExercises = CreateValidTrainingExercises();

        // Act
        var result = Training.Create(
            _validId,
            _validUserId,
            name!,
            _validMuscleGroups,
            _validDifficulty,
            trainingExercises,
            _validCreatedOnUtc,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithNullTrainingExercises_ShouldReturnFailure()
    {
        // Act
        var result = Training.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            null!,
            _validCreatedOnUtc,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithDefaultDateTime_ShouldReturnFailure()
    {
        // Arrange
        var trainingExercises = CreateValidTrainingExercises();
        var defaultDateTime = default(DateTime);

        // Act
        var result = Training.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            trainingExercises,
            defaultDateTime,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithNegativeDuration_ShouldReturnFailure(int duration)
    {
        // Arrange
        var trainingExercises = CreateValidTrainingExercises();

        // Act
        var result = Training.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            trainingExercises,
            _validCreatedOnUtc,
            duration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Create_WithNegativeRestSeconds_ShouldReturnFailure(int restSeconds)
    {
        // Arrange
        var trainingExercises = CreateValidTrainingExercises();

        // Act
        var result = Training.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            trainingExercises,
            _validCreatedOnUtc,
            _validDuration,
            restSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyMuscleGroups_ShouldReturnFailure()
    {
        // Arrange
        var trainingExercises = CreateValidTrainingExercises();
        var emptyMuscleGroups = new List<string>();

        // Act
        var result = Training.Create(
            _validId,
            _validUserId,
            _validName,
            emptyMuscleGroups,
            _validDifficulty,
            trainingExercises,
            _validCreatedOnUtc,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void UpdateDetails_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var training = CreateValidTraining();
        var newName = "Updated Training";
        var newMuscleGroups = new List<string> { "Back", "Biceps" };
        var newDifficulty = Difficulty.Hard;
        var newDuration = 60;
        var newRestSeconds = 120;
        var newDescription = "Updated description";

        // Act
        var result = training.UpdateDetails(
            newName,
            newMuscleGroups,
            newDifficulty,
            newDuration,
            newRestSeconds,
            newDescription
        );

        // Assert
        result.IsSuccess.Should().BeTrue();
        training.Name.Should().Be(newName);
        training.MuscleGroups.Should().BeEquivalentTo(newMuscleGroups);
        training.Difficulty.Should().Be(newDifficulty);
        training.Duration.Should().Be(newDuration);
        training.RestSeconds.Should().Be(newRestSeconds);
        training.Description.Should().Be(newDescription);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateDetails_WithInvalidName_ShouldReturnFailure(string? name)
    {
        // Arrange
        var training = CreateValidTraining();

        // Act
        var result = training.UpdateDetails(
            name!,
            _validMuscleGroups,
            _validDifficulty,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void UpdateDetails_WithNegativeDuration_ShouldReturnFailure(int duration)
    {
        // Arrange
        var training = CreateValidTraining();

        // Act
        var result = training.UpdateDetails(
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            duration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void UpdateDetails_WithNegativeRestSeconds_ShouldReturnFailure(int restSeconds)
    {
        // Arrange
        var training = CreateValidTraining();

        // Act
        var result = training.UpdateDetails(
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            _validDuration,
            restSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void UpdateDetails_WithEmptyMuscleGroups_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var emptyMuscleGroups = new List<string>();

        // Act
        var result = training.UpdateDetails(
            _validName,
            emptyMuscleGroups,
            _validDifficulty,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void UpdateExercises_WithValidExercises_ShouldReturnSuccess()
    {
        // Arrange
        var training = CreateValidTraining();
        var newExercises = CreateValidTrainingExercises();

        // Act
        var result = training.UpdateExercises(newExercises);

        // Assert
        result.IsSuccess.Should().BeTrue();
        training.TrainingExercises.Should().BeEquivalentTo(newExercises);
    }

    [Fact]
    public void UpdateExercises_WithEmptyExercises_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var emptyExercises = new List<TrainingExercise>();

        // Act
        var result = training.UpdateExercises(emptyExercises);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void RemoveExercise_WithValidExerciseId_ShouldReturnSuccess()
    {
        // Arrange
        var training = CreateValidTraining();
        var exerciseId = training.TrainingExercises.First().Exercise.Id;
        var originalCount = training.TrainingExercises.Count;

        // Act
        var result = training.RemoveExercise(exerciseId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        training.TrainingExercises.Should().HaveCount(originalCount - 1);
        training.TrainingExercises.Should().NotContain(te => te.Exercise.Id == exerciseId);
    }

    [Fact]
    public void RemoveExercise_WithEmptyExerciseId_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var emptyExerciseId = ExerciseId.Empty;

        // Act
        var result = training.RemoveExercise(emptyExerciseId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void RemoveExercise_WithNonExistentExerciseId_ShouldReturnSuccess()
    {
        // Arrange
        var training = CreateValidTraining();
        var nonExistentExerciseId = ExerciseId.Create(Guid.NewGuid());
        var originalCount = training.TrainingExercises.Count;

        // Act
        var result = training.RemoveExercise(nonExistentExerciseId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        training.TrainingExercises.Should().HaveCount(originalCount);
    }

    private Training CreateValidTraining()
    {
        var trainingExercises = CreateValidTrainingExercises();
        var result = Training.Create(
            _validId,
            _validUserId,
            _validName,
            _validMuscleGroups,
            _validDifficulty,
            trainingExercises,
            _validCreatedOnUtc,
            _validDuration,
            _validRestSeconds,
            _validDescription
        );

        return result.Value;
    }
}
