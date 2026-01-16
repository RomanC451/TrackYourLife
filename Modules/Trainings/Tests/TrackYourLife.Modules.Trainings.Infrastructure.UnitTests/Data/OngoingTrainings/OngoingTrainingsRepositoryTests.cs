using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Infrastructure.UnitTests.Data.OngoingTrainings;

public class OngoingTrainingsRepositoryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private OngoingTrainingsRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new OngoingTrainingsRepository(WriteDbContext);
    }

    [Fact]
    public async Task GetByTrainingIdAndNotFinishedAsync_WhenOngoingTrainingExists_ShouldReturnTraining()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(
            training: training,
            finishedOnUtc: null
        );
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByTrainingIdAndNotFinishedAsync(
                training.Id,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(ongoingTraining.Id);
            result.Training.Id.Should().Be(training.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByTrainingIdAndNotFinishedAsync_WhenOngoingTrainingIsFinished_ShouldReturnNull()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var finishedOnUtc = DateTime.UtcNow;
        var ongoingTraining = OngoingTrainingFaker.Generate(
            training: training,
            finishedOnUtc: null
        );
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        // Finish the training after it's tracked by EF Core
        ongoingTraining.Finish(finishedOnUtc, null);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByTrainingIdAndNotFinishedAsync(
                training.Id,
                CancellationToken.None
            );

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByTrainingIdAndNotFinishedAsync_WhenNoOngoingTrainingExists_ShouldReturnNull()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByTrainingIdAndNotFinishedAsync(
                training.Id,
                CancellationToken.None
            );

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetUnfinishedByUserIdAsync_WhenOngoingTrainingExists_ShouldReturnTraining()
    {
        // Arrange
        var userId = UserId.NewId();
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(
            userId: userId,
            training: training,
            finishedOnUtc: null
        );
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetUnfinishedByUserIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(ongoingTraining.Id);
            result.UserId.Should().Be(userId);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetUnfinishedByUserIdAsync_WhenOngoingTrainingIsFinished_ShouldReturnNull()
    {
        // Arrange
        var userId = UserId.NewId();
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(
            userId: userId,
            training: training,
            finishedOnUtc: DateTime.UtcNow
        );
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetUnfinishedByUserIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetUnfinishedByUserIdAsync_WhenNoOngoingTrainingExists_ShouldReturnNull()
    {
        // Arrange
        var userId = UserId.NewId();

        try
        {
            // Act
            var result = await _sut.GetUnfinishedByUserIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetUnfinishedByUserIdAsync_WhenOngoingTrainingExistsForDifferentUser_ShouldReturnNull()
    {
        // Arrange
        var userId1 = UserId.NewId();
        var userId2 = UserId.NewId();
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(
            userId: userId1,
            training: training,
            finishedOnUtc: null
        );
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetUnfinishedByUserIdAsync(userId2, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task AddAsync_WhenOngoingTrainingIsAdded_ShouldPersistToDatabase()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(training: training);

        try
        {
            // Act
            await _sut.AddAsync(ongoingTraining, CancellationToken.None);
            await WriteDbContext.SaveChangesAsync();

            // Assert
            var result = await WriteDbContext.OngoingTrainings
                .Include(ot => ot.Training)
                .ThenInclude(t => t.TrainingExercises)
                .ThenInclude(te => te.Exercise)
                .FirstOrDefaultAsync(ot => ot.Id == ongoingTraining.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(ongoingTraining.Id);
            result.Training.Id.Should().Be(training.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenOngoingTrainingExists_ShouldReturnTrainingWithRelations()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(training: training);
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(ongoingTraining.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(ongoingTraining.Id);
            result.Training.Should().NotBeNull();
            result.Training.Id.Should().Be(training.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenOngoingTrainingDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(ongoingTrainingId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task Update_WhenFinishingWithCaloriesBurned_ShouldPersistCaloriesBurned()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(
            training: training,
            finishedOnUtc: null
        );
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        var finishedOnUtc = DateTime.UtcNow;
        var caloriesBurned = 500;

        // Act
        ongoingTraining.Finish(finishedOnUtc, caloriesBurned);
        _sut.Update(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Assert
            WriteDbContext.ChangeTracker.Clear();
            var result = await WriteDbContext.OngoingTrainings
                .Include(ot => ot.Training)
                .ThenInclude(t => t.TrainingExercises)
                .ThenInclude(te => te.Exercise)
                .FirstOrDefaultAsync(ot => ot.Id == ongoingTraining.Id);

            result.Should().NotBeNull();
            result!.FinishedOnUtc.Should().BeCloseTo(finishedOnUtc, TimeSpan.FromSeconds(1));
            result.CaloriesBurned.Should().Be(caloriesBurned);
            result.IsFinished.Should().BeTrue();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenOngoingTrainingHasCaloriesBurned_ShouldReturnCaloriesBurned()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var caloriesBurned = 750;
        var ongoingTraining = OngoingTrainingFaker.Generate(
            training: training,
            finishedOnUtc: DateTime.UtcNow,
            caloriesBurned: caloriesBurned
        );
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(ongoingTraining.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(ongoingTraining.Id);
            result.CaloriesBurned.Should().Be(caloriesBurned);
            result.Training.Should().NotBeNull();
            result.Training.Id.Should().Be(training.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
