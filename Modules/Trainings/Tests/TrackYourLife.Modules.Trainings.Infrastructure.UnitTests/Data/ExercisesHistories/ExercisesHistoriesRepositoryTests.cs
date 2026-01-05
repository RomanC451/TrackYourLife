using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.ExercisesHistories;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Infrastructure.UnitTests.Data.ExercisesHistories;

public class ExercisesHistoriesRepositoryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private ExercisesHistoriesRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new ExercisesHistoriesRepository(WriteDbContext);
    }

    [Fact]
    public async Task GetByOngoingTrainingIdAndAreNotAppliedAsync_WhenHistoriesExist_ShouldReturnMatchingHistories()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        // Mark exercise as unchanged so EF Core doesn't try to insert it again
        WriteDbContext.Entry(exercise).State = EntityState.Unchanged;

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(training: training);
        var ongoingTrainingId = ongoingTraining.Id;
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        var otherOngoingTraining = OngoingTrainingFaker.Generate(training: training);
        await WriteDbContext.OngoingTrainings.AddAsync(otherOngoingTraining);
        await WriteDbContext.SaveChangesAsync();

        var history1 = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: ongoingTrainingId,
            exerciseId: exercise.Id
        );
        var history2 = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: ongoingTrainingId,
            exerciseId: exercise.Id
        );
        var history3 = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: otherOngoingTraining.Id,
            exerciseId: exercise.Id
        );

        await WriteDbContext.ExerciseHistories.AddAsync(history1);
        await WriteDbContext.ExerciseHistories.AddAsync(history2);
        await WriteDbContext.ExerciseHistories.AddAsync(history3);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByOngoingTrainingIdAndAreNotAppliedAsync(
                ongoingTrainingId,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(h => h.Id == history1.Id);
            result.Should().Contain(h => h.Id == history2.Id);
            result.Should().NotContain(h => h.Id == history3.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByOngoingTrainingIdAndAreNotAppliedAsync_WhenNoHistoriesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var ongoingTrainingId = OngoingTrainingId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByOngoingTrainingIdAndAreNotAppliedAsync(
                ongoingTrainingId,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByOngoingTrainingIdAndAreNotAppliedAsync_ShouldReturnHistoriesOrderedByCreatedOnUtcDescending()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        var now = DateTime.UtcNow;

        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        // Mark exercise as unchanged so EF Core doesn't try to insert it again
        WriteDbContext.Entry(exercise).State = EntityState.Unchanged;

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(training: training);
        var ongoingTrainingId = ongoingTraining.Id;
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        var history1 = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: ongoingTrainingId,
            exerciseId: exercise.Id,
            createdOnUtc: now.AddDays(-2)
        );
        var history2 = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: ongoingTrainingId,
            exerciseId: exercise.Id,
            createdOnUtc: now.AddDays(-1)
        );
        var history3 = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: ongoingTrainingId,
            exerciseId: exercise.Id,
            createdOnUtc: now
        );

        await WriteDbContext.ExerciseHistories.AddAsync(history1);
        await WriteDbContext.ExerciseHistories.AddAsync(history2);
        await WriteDbContext.ExerciseHistories.AddAsync(history3);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByOngoingTrainingIdAndAreNotAppliedAsync(
                ongoingTrainingId,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeInDescendingOrder(h => h.CreatedOnUtc);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task AddAsync_WhenHistoryIsAdded_ShouldPersistToDatabase()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        // Mark exercise as unchanged so EF Core doesn't try to insert it again
        WriteDbContext.Entry(exercise).State = EntityState.Unchanged;

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(training: training);
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        var history = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: ongoingTraining.Id,
            exerciseId: exercise.Id
        );

        try
        {
            // Act
            await _sut.AddAsync(history, CancellationToken.None);
            await WriteDbContext.SaveChangesAsync();

            // Assert
            var result = await WriteDbContext.ExerciseHistories.FindAsync(history.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(history.Id);
            result.ExerciseId.Should().Be(exercise.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenHistoryExists_ShouldReturnHistory()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        // Mark exercise as unchanged so EF Core doesn't try to insert it again
        WriteDbContext.Entry(exercise).State = EntityState.Unchanged;

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var ongoingTraining = OngoingTrainingFaker.Generate(training: training);
        await WriteDbContext.OngoingTrainings.AddAsync(ongoingTraining);
        await WriteDbContext.SaveChangesAsync();

        var history = ExerciseHistoryFaker.GenerateEntity(
            ongoingTrainingId: ongoingTraining.Id,
            exerciseId: exercise.Id
        );
        await WriteDbContext.ExerciseHistories.AddAsync(history);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(history.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(history.Id);
            result.ExerciseId.Should().Be(exercise.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenHistoryDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var historyId = ExerciseHistoryId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(historyId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
