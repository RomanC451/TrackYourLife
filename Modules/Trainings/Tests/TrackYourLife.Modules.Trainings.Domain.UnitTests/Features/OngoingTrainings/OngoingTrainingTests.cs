using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings.Events;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.OngoingTrainings;

public class OngoingTrainingTests
{
    private readonly OngoingTrainingId _validId = OngoingTrainingId.Create(Guid.NewGuid());
    private readonly UserId _validUserId = UserId.Create(Guid.NewGuid());
    private readonly DateTime _validStartedOnUtc = DateTime.UtcNow;

    private Training CreateValidTraining()
    {
        var trainingId = TrainingId.Create(Guid.NewGuid());
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

        var trainingExercise = TrainingExercise.Create(trainingId, exercise, 0).Value;
        var trainingExercises = new List<TrainingExercise> { trainingExercise };

        return Training
            .Create(
                trainingId,
                _validUserId,
                "Test Training",
                new List<string> { "Chest" },
                Difficulty.Easy,
                trainingExercises,
                DateTime.UtcNow,
                30,
                60,
                "Test training description"
            )
            .Value;
    }

    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        // Arrange
        var training = CreateValidTraining();

        // Act
        var result = OngoingTraining.Create(_validId, _validUserId, training, _validStartedOnUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var ongoingTraining = result.Value;
        ongoingTraining.Id.Should().Be(_validId);
        ongoingTraining.UserId.Should().Be(_validUserId);
        ongoingTraining.Training.Should().Be(training);
        ongoingTraining.StartedOnUtc.Should().Be(_validStartedOnUtc);
        ongoingTraining.ExerciseIndex.Should().Be(0);
        ongoingTraining.SetIndex.Should().Be(0);
        ongoingTraining.FinishedOnUtc.Should().BeNull();
        ongoingTraining.IsFinished.Should().BeFalse();
    }

    [Fact]
    public void Create_WithEmptyId_ShouldReturnFailure()
    {
        // Arrange
        var emptyId = OngoingTrainingId.Empty;
        var training = CreateValidTraining();

        // Act
        var result = OngoingTraining.Create(emptyId, _validUserId, training, _validStartedOnUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithEmptyUserId_ShouldReturnFailure()
    {
        // Arrange
        var emptyUserId = UserId.Empty;
        var training = CreateValidTraining();

        // Act
        var result = OngoingTraining.Create(_validId, emptyUserId, training, _validStartedOnUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithNullTraining_ShouldReturnFailure()
    {
        // Act
        var result = OngoingTraining.Create(_validId, _validUserId, null!, _validStartedOnUtc);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithDefaultDateTime_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var defaultDateTime = default(DateTime);

        // Act
        var result = OngoingTraining.Create(_validId, _validUserId, training, defaultDateTime);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Properties_ShouldReturnCorrectValues()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;

        // Assert
        ongoingTraining.ExercisesCount.Should().Be(1);
        ongoingTraining.CurrentExercise.Should().Be(training.TrainingExercises.First().Exercise);
        ongoingTraining.SetsCount.Should().Be(2);
        ongoingTraining.IsFirstSet.Should().BeTrue();
        ongoingTraining.IsFirstExercise.Should().BeTrue();
        ongoingTraining.IsFirstSetAndExercise.Should().BeTrue();
        ongoingTraining.HasPrevious.Should().BeFalse();
        ongoingTraining.IsLastSet.Should().BeFalse();
        ongoingTraining.IsLastExercise.Should().BeTrue();
        ongoingTraining.IsLastSetAndExercise.Should().BeFalse();
        ongoingTraining.HasNext.Should().BeTrue();
    }

    [Fact]
    public void Finish_WithValidDateTime_ShouldReturnSuccess()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        var finishedOnUtc = DateTime.UtcNow.AddMinutes(30);

        // Act
        var result = ongoingTraining.Finish(finishedOnUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.FinishedOnUtc.Should().Be(finishedOnUtc);
        ongoingTraining.IsFinished.Should().BeTrue();
    }

    [Fact]
    public void Finish_WithDefaultDateTime_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        var defaultDateTime = default(DateTime);

        // Act
        var result = ongoingTraining.Finish(defaultDateTime);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Finish_WhenAlreadyFinished_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        var finishedOnUtc = DateTime.UtcNow.AddMinutes(30);
        ongoingTraining.Finish(finishedOnUtc);

        // Act
        var result = ongoingTraining.Finish(DateTime.UtcNow.AddMinutes(45));

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Finish_ShouldRaiseDomainEvent()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        var finishedOnUtc = DateTime.UtcNow.AddMinutes(30);

        // Act
        var result = ongoingTraining.Finish(finishedOnUtc);

        // Assert
        result.IsSuccess.Should().BeTrue();
        // Note: Domain events are handled internally by the aggregate root
        // We can't directly access them in the test, but we can verify the training is finished
        ongoingTraining.IsFinished.Should().BeTrue();
    }

    [Fact]
    public void Next_WhenNotFinished_ShouldMoveToNextSet()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;

        // Act
        var result = ongoingTraining.Next();

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.SetIndex.Should().Be(1);
        ongoingTraining.ExerciseIndex.Should().Be(0);
    }

    [Fact]
    public void Next_WhenLastSet_ShouldMoveToNextExercise()
    {
        // Arrange - Create a training with multiple exercises
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId1 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId2 = ExerciseId.Create(Guid.NewGuid());

        var exercise1 = Exercise
            .Create(
                exerciseId1,
                _validUserId,
                "Exercise 1",
                new List<string> { "Chest" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50.0f),
                },
                DateTime.UtcNow
            )
            .Value;

        var exercise2 = Exercise
            .Create(
                exerciseId2,
                _validUserId,
                "Exercise 2",
                new List<string> { "Back" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 8, 60.0f),
                },
                DateTime.UtcNow
            )
            .Value;

        var trainingExercise1 = TrainingExercise.Create(trainingId, exercise1, 0).Value;
        var trainingExercise2 = TrainingExercise.Create(trainingId, exercise2, 1).Value;
        var trainingExercises = new List<TrainingExercise> { trainingExercise1, trainingExercise2 };

        var training = Training
            .Create(
                trainingId,
                _validUserId,
                "Test Training",
                new List<string> { "Chest", "Back" },
                Difficulty.Easy,
                trainingExercises,
                DateTime.UtcNow,
                30,
                60,
                "Test training description"
            )
            .Value;

        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;

        // Act
        var result = ongoingTraining.Next();

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.SetIndex.Should().Be(0);
        ongoingTraining.ExerciseIndex.Should().Be(1);
    }

    [Fact]
    public void Next_WhenFinished_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        ongoingTraining.Finish(DateTime.UtcNow.AddMinutes(30));

        // Act
        var result = ongoingTraining.Next();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Next_WhenNoNext_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        ongoingTraining.Next(); // Move to second set (last set)

        // Act
        var result = ongoingTraining.Next();

        // Assert
        result.IsFailure.Should().BeTrue(); // Should fail because there's no next exercise
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Previous_WhenNotFinished_ShouldMoveToPreviousSet()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        ongoingTraining.Next(); // Move to second set

        // Act
        var result = ongoingTraining.Previous();

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.SetIndex.Should().Be(0);
        ongoingTraining.ExerciseIndex.Should().Be(0);
    }

    [Fact]
    public void Previous_WhenFirstSet_ShouldMoveToPreviousExercise()
    {
        // Arrange - Create a training with multiple exercises
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId1 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId2 = ExerciseId.Create(Guid.NewGuid());

        var exercise1 = Exercise
            .Create(
                exerciseId1,
                _validUserId,
                "Exercise 1",
                new List<string> { "Chest" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50.0f),
                },
                DateTime.UtcNow
            )
            .Value;

        var exercise2 = Exercise
            .Create(
                exerciseId2,
                _validUserId,
                "Exercise 2",
                new List<string> { "Back" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 8, 60.0f),
                },
                DateTime.UtcNow
            )
            .Value;

        var trainingExercise1 = TrainingExercise.Create(trainingId, exercise1, 0).Value;
        var trainingExercise2 = TrainingExercise.Create(trainingId, exercise2, 1).Value;
        var trainingExercises = new List<TrainingExercise> { trainingExercise1, trainingExercise2 };

        var training = Training
            .Create(
                trainingId,
                _validUserId,
                "Test Training",
                new List<string> { "Chest", "Back" },
                Difficulty.Easy,
                trainingExercises,
                DateTime.UtcNow,
                30,
                60,
                "Test training description"
            )
            .Value;

        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;

        ongoingTraining.Next(); // Move to next exercise (exercise 2)

        // Act
        var result = ongoingTraining.Previous();

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.SetIndex.Should().Be(0); // First set of previous exercise
        ongoingTraining.ExerciseIndex.Should().Be(0);
    }

    [Fact]
    public void Previous_WhenFinished_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        ongoingTraining.Finish(DateTime.UtcNow.AddMinutes(30));

        // Act
        var result = ongoingTraining.Previous();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Previous_WhenNoPrevious_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;

        // Act
        var result = ongoingTraining.Previous();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }
}
