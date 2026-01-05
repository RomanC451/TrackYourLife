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

public class OngoingTrainingsQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private OngoingTrainingsQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new OngoingTrainingsQuery(ReadDbContext);
    }

    [Fact]
    public async Task GetUnfinishedByUserIdAsync_WhenOngoingTrainingExists_ShouldReturnReadModel()
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
            result.FinishedOnUtc.Should().BeNull();
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

        var finishedOnUtc = DateTime.UtcNow;
        var ongoingTraining = OngoingTrainingFaker.Generate(
            userId: userId,
            training: training,
            finishedOnUtc: null
        );
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        // Finish the training after it's tracked by EF Core
        ongoingTraining.Finish(finishedOnUtc);
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
    public async Task IsTrainingOngoingAsync_WhenOngoingTrainingExists_ShouldReturnTrue()
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
            var result = await _sut.IsTrainingOngoingAsync(training.Id, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task IsTrainingOngoingAsync_WhenOngoingTrainingIsFinished_ShouldReturnFalse()
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
        ongoingTraining.Finish(finishedOnUtc);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.IsTrainingOngoingAsync(training.Id, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task IsTrainingOngoingAsync_WhenNoOngoingTrainingExists_ShouldReturnFalse()
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
            var result = await _sut.IsTrainingOngoingAsync(training.Id, CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenOngoingTrainingExists_ShouldReturnReadModelWithTraining()
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
}
