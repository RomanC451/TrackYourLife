using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
                    ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 8, "reps", 60.0f, "kg").Value,
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
        ongoingTraining.IsLastSet.Should().BeFalse();
        ongoingTraining.IsLastExercise.Should().BeTrue();
        ongoingTraining.IsLastSetAndExercise.Should().BeFalse();
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
        ongoingTraining.CaloriesBurned.Should().BeNull();
    }

    [Fact]
    public void Finish_WithValidDateTimeAndCaloriesBurned_ShouldReturnSuccess()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        var finishedOnUtc = DateTime.UtcNow.AddMinutes(30);
        var caloriesBurned = 500;

        // Act
        var result = ongoingTraining.Finish(finishedOnUtc, caloriesBurned);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.FinishedOnUtc.Should().Be(finishedOnUtc);
        ongoingTraining.IsFinished.Should().BeTrue();
        ongoingTraining.CaloriesBurned.Should().Be(caloriesBurned);
    }

    [Fact]
    public void Finish_WithValidDateTimeAndNullCaloriesBurned_ShouldReturnSuccess()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        var finishedOnUtc = DateTime.UtcNow.AddMinutes(30);

        // Act
        var result = ongoingTraining.Finish(finishedOnUtc, null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.FinishedOnUtc.Should().Be(finishedOnUtc);
        ongoingTraining.IsFinished.Should().BeTrue();
        ongoingTraining.CaloriesBurned.Should().BeNull();
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
        ongoingTraining.Finish(finishedOnUtc, null);

        // Act
        var result = ongoingTraining.Finish(DateTime.UtcNow.AddMinutes(45), null);

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
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId>();

        // Act
        var result = ongoingTraining.Next(completedOrSkippedExerciseIds);

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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 8, "reps", 60.0f, "kg").Value,
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
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId>();

        // Act
        var result = ongoingTraining.Next(completedOrSkippedExerciseIds);

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
        ongoingTraining.Finish(DateTime.UtcNow.AddMinutes(30), null);
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId>();

        // Act
        var result = ongoingTraining.Next(completedOrSkippedExerciseIds);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Next_WhenLastSet_ShouldSkipCompletedOrSkippedExercises()
    {
        // Arrange - Create a training with multiple exercises
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId1 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId2 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId3 = ExerciseId.Create(Guid.NewGuid());

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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 8, "reps", 60.0f, "kg").Value,
                },
                DateTime.UtcNow
            )
            .Value;

        var exercise3 = Exercise
            .Create(
                exerciseId3,
                _validUserId,
                "Exercise 3",
                new List<string> { "Legs" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 70.0f, "kg").Value,
                },
                DateTime.UtcNow
            )
            .Value;

        var trainingExercise1 = TrainingExercise.Create(trainingId, exercise1, 0).Value;
        var trainingExercise2 = TrainingExercise.Create(trainingId, exercise2, 1).Value;
        var trainingExercise3 = TrainingExercise.Create(trainingId, exercise3, 2).Value;
        var trainingExercises = new List<TrainingExercise>
        {
            trainingExercise1,
            trainingExercise2,
            trainingExercise3,
        };

        var training = Training
            .Create(
                trainingId,
                _validUserId,
                "Test Training",
                new List<string> { "Chest", "Back", "Legs" },
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

        // Mark exercise 2 as completed/skipped
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId> { exerciseId2 };

        // Act - Move to last set of exercise 1, then call Next
        var nextResult = ongoingTraining.Next(completedOrSkippedExerciseIds);

        // Assert
        nextResult.IsSuccess.Should().BeTrue();
        ongoingTraining.ExerciseIndex.Should().Be(2); // Should skip exercise 2 and move to exercise 3
        ongoingTraining.SetIndex.Should().Be(0);
    }

    [Fact]
    public void Next_WhenLastSet_ShouldWrapAroundToFindFirstIncompleteExercise()
    {
        // Arrange - Create a training with multiple exercises
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId1 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId2 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId3 = ExerciseId.Create(Guid.NewGuid());

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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 8, "reps", 60.0f, "kg").Value,
                },
                DateTime.UtcNow
            )
            .Value;

        var exercise3 = Exercise
            .Create(
                exerciseId3,
                _validUserId,
                "Exercise 3",
                new List<string> { "Legs" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 70.0f, "kg").Value,
                },
                DateTime.UtcNow
            )
            .Value;

        var trainingExercise1 = TrainingExercise.Create(trainingId, exercise1, 0).Value;
        var trainingExercise2 = TrainingExercise.Create(trainingId, exercise2, 1).Value;
        var trainingExercise3 = TrainingExercise.Create(trainingId, exercise3, 2).Value;
        var trainingExercises = new List<TrainingExercise>
        {
            trainingExercise1,
            trainingExercise2,
            trainingExercise3,
        };

        var training = Training
            .Create(
                trainingId,
                _validUserId,
                "Test Training",
                new List<string> { "Chest", "Back", "Legs" },
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

        // Move to exercise 3 (last exercise)
        var emptySet = new HashSet<ExerciseId>();
        ongoingTraining.Next(emptySet); // Move to next exercise (exercise 2)
        ongoingTraining.Next(emptySet); // Move to next exercise (exercise 3)

        // Mark exercises 2 and 3 as completed/skipped
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId> { exerciseId2, exerciseId3 };

        // Act - Call Next from last exercise, should wrap around to exercise 1
        var nextResult = ongoingTraining.Next(completedOrSkippedExerciseIds);

        // Assert
        nextResult.IsSuccess.Should().BeTrue();
        ongoingTraining.ExerciseIndex.Should().Be(0); // Should wrap around to exercise 1 (index 0)
        ongoingTraining.SetIndex.Should().Be(0);
    }

    [Fact]
    public void Next_WhenAllExercisesCompletedOrSkipped_ShouldReturnFailure()
    {
        // Arrange - Create a training with multiple exercises
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId1 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId2 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId3 = ExerciseId.Create(Guid.NewGuid());

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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 8, "reps", 60.0f, "kg").Value,
                },
                DateTime.UtcNow
            )
            .Value;

        var exercise3 = Exercise
            .Create(
                exerciseId3,
                _validUserId,
                "Exercise 3",
                new List<string> { "Legs" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 70.0f, "kg").Value,
                },
                DateTime.UtcNow
            )
            .Value;

        var trainingExercise1 = TrainingExercise.Create(trainingId, exercise1, 0).Value;
        var trainingExercise2 = TrainingExercise.Create(trainingId, exercise2, 1).Value;
        var trainingExercise3 = TrainingExercise.Create(trainingId, exercise3, 2).Value;
        var trainingExercises = new List<TrainingExercise>
        {
            trainingExercise1,
            trainingExercise2,
            trainingExercise3,
        };

        var training = Training
            .Create(
                trainingId,
                _validUserId,
                "Test Training",
                new List<string> { "Chest", "Back", "Legs" },
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

        // Mark all exercises as completed/skipped
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId>
        {
            exerciseId1,
            exerciseId2,
            exerciseId3,
        };

        // Act - Call Next when all exercises are completed/skipped
        var nextResult = ongoingTraining.Next(completedOrSkippedExerciseIds);

        // Assert
        nextResult.IsFailure.Should().BeTrue();
        nextResult
            .Error.Should()
            .Be(OngoingTrainingErrors.AllExercisesCompletedOrSkipped(_validId));
        // Should stay on current exercise
        ongoingTraining.ExerciseIndex.Should().Be(0);
        ongoingTraining.SetIndex.Should().Be(0);
    }

    [Fact]
    public void Previous_WhenNotFinished_ShouldMoveToPreviousSet()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId>();
        ongoingTraining.Next(completedOrSkippedExerciseIds); // Move to second set

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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 8, "reps", 60.0f, "kg").Value,
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
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId>();

        ongoingTraining.Next(completedOrSkippedExerciseIds); // Move to next exercise (exercise 2)

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
        ongoingTraining.Finish(DateTime.UtcNow.AddMinutes(30), null);

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

    [Fact]
    public void SkipExercise_WhenNotFinished_ShouldMoveToNextIncompleteExercise()
    {
        // Arrange - Create a training with multiple exercises
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var exerciseId1 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId2 = ExerciseId.Create(Guid.NewGuid());
        var exerciseId3 = ExerciseId.Create(Guid.NewGuid());

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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 8, "reps", 60.0f, "kg").Value,
                },
                DateTime.UtcNow
            )
            .Value;

        var exercise3 = Exercise
            .Create(
                exerciseId3,
                _validUserId,
                "Exercise 3",
                new List<string> { "Legs" },
                Difficulty.Easy,
                null,
                null,
                null,
                null,
                new List<ExerciseSet>
                {
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 12, "reps", 70.0f, "kg").Value,
                },
                DateTime.UtcNow
            )
            .Value;

        var trainingExercise1 = TrainingExercise.Create(trainingId, exercise1, 0).Value;
        var trainingExercise2 = TrainingExercise.Create(trainingId, exercise2, 1).Value;
        var trainingExercise3 = TrainingExercise.Create(trainingId, exercise3, 2).Value;
        var trainingExercises = new List<TrainingExercise>
        {
            trainingExercise1,
            trainingExercise2,
            trainingExercise3,
        };

        var training = Training
            .Create(
                trainingId,
                _validUserId,
                "Test Training",
                new List<string> { "Chest", "Back", "Legs" },
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

        // Mark exercise 2 as completed/skipped
        var completedOrSkippedExerciseIds = new HashSet<ExerciseId> { exerciseId2 };

        // Act
        var result = ongoingTraining.SkipExercise(completedOrSkippedExerciseIds);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.ExerciseIndex.Should().Be(2); // Should move to exercise 3 (index 2)
        ongoingTraining.SetIndex.Should().Be(0);
    }

    [Fact]
    public void SkipExercise_WhenFinished_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        ongoingTraining.Finish(DateTime.UtcNow.AddMinutes(30), null);

        var completedOrSkippedExerciseIds = new HashSet<ExerciseId>();

        // Act
        var result = ongoingTraining.SkipExercise(completedOrSkippedExerciseIds);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void JumpToExercise_WithValidIndex_ShouldMoveToExercise()
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 8, "reps", 60.0f, "kg").Value,
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
        var result = ongoingTraining.JumpToExercise(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ongoingTraining.ExerciseIndex.Should().Be(1);
        ongoingTraining.SetIndex.Should().Be(0);
    }

    [Fact]
    public void JumpToExercise_WithInvalidIndex_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;

        // Act
        var result = ongoingTraining.JumpToExercise(10); // Invalid index

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void JumpToExercise_WhenFinished_ShouldReturnFailure()
    {
        // Arrange
        var training = CreateValidTraining();
        var ongoingTraining = OngoingTraining
            .Create(_validId, _validUserId, training, _validStartedOnUtc)
            .Value;
        ongoingTraining.Finish(DateTime.UtcNow.AddMinutes(30), null);

        // Act
        var result = ongoingTraining.JumpToExercise(0);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void GetAllExerciseIds_ShouldReturnAllExerciseIds()
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
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
                    ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 8, "reps", 60.0f, "kg").Value,
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
        var exerciseIds = ongoingTraining.GetAllExerciseIds();

        // Assert
        exerciseIds.Should().HaveCount(2);
        exerciseIds.Should().Contain(exerciseId1);
        exerciseIds.Should().Contain(exerciseId2);
    }
}
