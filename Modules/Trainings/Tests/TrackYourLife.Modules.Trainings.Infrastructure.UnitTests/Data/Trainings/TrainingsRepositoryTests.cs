using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Infrastructure.UnitTests.Data.Trainings;

public class TrainingsRepositoryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private TrainingsRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new TrainingsRepository(WriteDbContext);
    }

    [Fact]
    public async Task GetThatContainsExerciseAsync_WhenTrainingsExist_ShouldReturnMatchingTrainings()
    {
        // Arrange
        var exercise1 = ExerciseFaker.Generate();
        var exercise2 = ExerciseFaker.Generate();
        var exercise3 = ExerciseFaker.Generate();

        await WriteDbContext.Exercises.AddAsync(exercise1);
        await WriteDbContext.Exercises.AddAsync(exercise2);
        await WriteDbContext.Exercises.AddAsync(exercise3);
        await WriteDbContext.SaveChangesAsync();

        var training1 = TrainingFaker.Generate(exercises: new List<Exercise> { exercise1, exercise2 });
        var training2 = TrainingFaker.Generate(exercises: new List<Exercise> { exercise2, exercise3 });
        var training3 = TrainingFaker.Generate(exercises: new List<Exercise> { exercise3 });

        await WriteDbContext.Trainings.AddAsync(training1);
        await WriteDbContext.Trainings.AddAsync(training2);
        await WriteDbContext.Trainings.AddAsync(training3);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetThatContainsExerciseAsync(exercise2.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Id == training1.Id);
            result.Should().Contain(t => t.Id == training2.Id);
            result.Should().NotContain(t => t.Id == training3.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetThatContainsExerciseAsync_WhenNoTrainingsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();

        try
        {
            // Act
            var result = await _sut.GetThatContainsExerciseAsync(exerciseId, CancellationToken.None);

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
    public async Task GetThatContainsExerciseAsync_WhenExerciseDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });

        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        var nonExistentExerciseId = ExerciseId.NewId();

        try
        {
            // Act
            var result = await _sut.GetThatContainsExerciseAsync(
                nonExistentExerciseId,
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
    public async Task AddAsync_WhenTrainingIsAdded_ShouldPersistToDatabase()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(exercises: new List<Exercise> { exercise });

        try
        {
            // Act
            await _sut.AddAsync(training, CancellationToken.None);
            await WriteDbContext.SaveChangesAsync();

            // Assert
            var result = await WriteDbContext.Trainings
                .Include(t => t.TrainingExercises)
                .ThenInclude(te => te.Exercise)
                .FirstOrDefaultAsync(t => t.Id == training.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(training.Id);
            result.Name.Should().Be(training.Name);
            result.TrainingExercises.Should().HaveCount(1);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenTrainingExists_ShouldReturnTrainingWithExercises()
    {
        // Arrange
        var exercise1 = ExerciseFaker.Generate();
        var exercise2 = ExerciseFaker.Generate();

        await WriteDbContext.Exercises.AddAsync(exercise1);
        await WriteDbContext.Exercises.AddAsync(exercise2);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(
            exercises: new List<Exercise> { exercise1, exercise2 }
        );
        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(training.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(training.Id);
            result.Name.Should().Be(training.Name);
            result.TrainingExercises.Should().HaveCount(2);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenTrainingDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var trainingId = TrainingId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(trainingId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
