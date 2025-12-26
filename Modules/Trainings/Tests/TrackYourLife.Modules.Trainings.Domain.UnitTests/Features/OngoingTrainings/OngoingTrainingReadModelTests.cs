using FluentAssertions;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Domain.UnitTests.Features.OngoingTrainings;

public class OngoingTrainingReadModelTests
{
    [Fact]
    public void OngoingTrainingReadModel_ShouldImplementIReadModel()
    {
        // Arrange & Act
        var readModelType = typeof(OngoingTrainingReadModel);
        var interfaceType = typeof(IReadModel<OngoingTrainingId>);

        // Assert
        readModelType.Should().BeAssignableTo(interfaceType);
    }

    [Fact]
    public void OngoingTrainingReadModel_ShouldBeRecord()
    {
        // Arrange & Act
        var readModelType = typeof(OngoingTrainingReadModel);

        // Assert
        readModelType.IsClass.Should().BeTrue();
        readModelType.IsSealed.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldSetProperties()
    {
        // Arrange
        var id = OngoingTrainingId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var exerciseIndex = 1;
        var setIndex = 2;
        var startedOnUtc = DateTime.UtcNow;
        var finishedOnUtc = DateTime.UtcNow.AddMinutes(30);

        // Act
        var readModel = new OngoingTrainingReadModel(
            id,
            userId,
            exerciseIndex,
            setIndex,
            startedOnUtc,
            finishedOnUtc
        );

        // Assert
        readModel.Id.Should().Be(id);
        readModel.UserId.Should().Be(userId);
        readModel.ExerciseIndex.Should().Be(exerciseIndex);
        readModel.SetIndex.Should().Be(setIndex);
        readModel.StartedOnUtc.Should().Be(startedOnUtc);
        readModel.FinishedOnUtc.Should().Be(finishedOnUtc);
    }

    [Fact]
    public void Constructor_WithNullFinishedOnUtc_ShouldSetNull()
    {
        // Arrange
        var id = OngoingTrainingId.Create(Guid.NewGuid());
        var userId = UserId.Create(Guid.NewGuid());
        var exerciseIndex = 0;
        var setIndex = 0;
        var startedOnUtc = DateTime.UtcNow;

        // Act
        var readModel = new OngoingTrainingReadModel(
            id,
            userId,
            exerciseIndex,
            setIndex,
            startedOnUtc,
            null
        );

        // Assert
        readModel.FinishedOnUtc.Should().BeNull();
        readModel.IsFinished.Should().BeFalse();
    }

    [Fact]
    public void IsFinished_WhenFinishedOnUtcIsNull_ShouldReturnFalse()
    {
        // Arrange
        var readModel = new OngoingTrainingReadModel(
            OngoingTrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            0,
            0,
            DateTime.UtcNow,
            null
        );

        // Act & Assert
        readModel.IsFinished.Should().BeFalse();
    }

    [Fact]
    public void IsFinished_WhenFinishedOnUtcHasValue_ShouldReturnTrue()
    {
        // Arrange
        var readModel = new OngoingTrainingReadModel(
            OngoingTrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            0,
            0,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(30)
        );

        // Act & Assert
        readModel.IsFinished.Should().BeTrue();
    }

    [Fact]
    public void Properties_WithTrainingExercises_ShouldCalculateCorrectly()
    {
        // Arrange
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        var exercise = new ExerciseReadModel(
            exerciseId,
            UserId.Create(Guid.NewGuid()),
            "Test Exercise",
            new List<string> { "Chest" },
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            null
        )
        {
            ExerciseSets = new List<ExerciseSet>
            {
                new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50.0f),
                new WeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 8, 60.0f),
                new WeightBasedExerciseSet(Guid.NewGuid(), "Set 3", 2, 6, 70.0f),
            },
        };

        var trainingExercise = new TrainingExerciseReadModel(
            TrainingId.Create(Guid.NewGuid()),
            exerciseId,
            0
        )
        {
            Exercise = exercise,
        };

        var training = new TrainingReadModel(
            TrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            "Test description",
            DateTime.UtcNow,
            30,
            60,
            null
        )
        {
            TrainingExercises = new List<TrainingExerciseReadModel> { trainingExercise },
        };

        var readModel = new OngoingTrainingReadModel(
            OngoingTrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            0,
            1,
            DateTime.UtcNow,
            null
        )
        {
            Training = training,
        };

        // Act & Assert
        readModel.ExercisesCount.Should().Be(1);
        readModel.CurrentExercise.Should().Be(exercise);
        readModel.SetsCount.Should().Be(3);
        readModel.IsFirstSet.Should().BeFalse();
        readModel.IsFirstExercise.Should().BeTrue();
        readModel.IsFirstSetAndExercise.Should().BeFalse();
        readModel.HasPrevious.Should().BeTrue();
        readModel.IsLastSet.Should().BeFalse();
        readModel.IsLastExercise.Should().BeTrue();
        readModel.IsLastSetAndExercise.Should().BeFalse();
        readModel.HasNext.Should().BeTrue();
    }

    [Fact]
    public void Properties_AtFirstSetAndExercise_ShouldCalculateCorrectly()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var training = CreateTestTraining(exercise);
        var readModel = new OngoingTrainingReadModel(
            OngoingTrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            0,
            0,
            DateTime.UtcNow,
            null
        )
        {
            Training = training,
        };

        // Act & Assert
        readModel.IsFirstSet.Should().BeTrue();
        readModel.IsFirstExercise.Should().BeTrue();
        readModel.IsFirstSetAndExercise.Should().BeTrue();
        readModel.HasPrevious.Should().BeFalse();
    }

    [Fact]
    public void Properties_AtLastSetAndExercise_ShouldCalculateCorrectly()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var training = CreateTestTraining(exercise);
        var readModel = new OngoingTrainingReadModel(
            OngoingTrainingId.Create(Guid.NewGuid()),
            UserId.Create(Guid.NewGuid()),
            0,
            2, // Last set (index 2 of 3 sets)
            DateTime.UtcNow,
            null
        )
        {
            Training = training,
        };

        // Act & Assert
        readModel.IsLastSet.Should().BeTrue();
        readModel.IsLastExercise.Should().BeTrue();
        readModel.IsLastSetAndExercise.Should().BeTrue();
        readModel.HasNext.Should().BeFalse();
    }

    private ExerciseReadModel CreateTestExercise()
    {
        var exerciseId = ExerciseId.Create(Guid.NewGuid());
        return new ExerciseReadModel(
            exerciseId,
            UserId.Create(Guid.NewGuid()),
            "Test Exercise",
            new List<string> { "Chest" },
            Difficulty.Easy,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            null
        )
        {
            ExerciseSets = new List<ExerciseSet>
            {
                new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 10, 50.0f),
                new WeightBasedExerciseSet(Guid.NewGuid(), "Set 2", 1, 8, 60.0f),
                new WeightBasedExerciseSet(Guid.NewGuid(), "Set 3", 2, 6, 70.0f),
            },
        };
    }

    private TrainingReadModel CreateTestTraining(ExerciseReadModel exercise)
    {
        var trainingId = TrainingId.Create(Guid.NewGuid());
        var trainingExercise = new TrainingExerciseReadModel(trainingId, exercise.Id, 0)
        {
            Exercise = exercise,
        };

        return new TrainingReadModel(
            trainingId,
            UserId.Create(Guid.NewGuid()),
            "Test Training",
            new List<string> { "Chest" },
            Difficulty.Easy,
            "Test description",
            DateTime.UtcNow,
            30,
            60,
            null
        )
        {
            TrainingExercises = new List<TrainingExerciseReadModel> { trainingExercise },
        };
    }
}
