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

public class TrainingsQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private TrainingsQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new TrainingsQuery(ReadDbContext);
    }

    [Fact]
    public async Task GetTrainingsByUserIdAsync_WhenTrainingsExist_ShouldReturnMatchingTrainings()
    {
        // Arrange
        var userId = UserId.NewId();
        var exercise = ExerciseFaker.Generate();

        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training1 = TrainingFaker.Generate(userId: userId, exercises: new List<Exercise> { exercise });
        var training2 = TrainingFaker.Generate(userId: userId, exercises: new List<Exercise> { exercise });
        var training3 = TrainingFaker.Generate(exercises: new List<Exercise> { exercise }); // Different user

        await WriteDbContext.Trainings.AddAsync(training1);
        await WriteDbContext.Trainings.AddAsync(training2);
        await WriteDbContext.Trainings.AddAsync(training3);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetTrainingsByUserIdAsync(userId, CancellationToken.None);

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
    public async Task GetTrainingsByUserIdAsync_WhenNoTrainingsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();

        try
        {
            // Act
            var result = await _sut.GetTrainingsByUserIdAsync(userId, CancellationToken.None);

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
    public async Task ExistsByUserIdAndNameAsync_WhenTrainingExists_ShouldReturnTrue()
    {
        // Arrange
        var userId = UserId.NewId();
        var trainingName = "Test Training";
        var exercise = ExerciseFaker.Generate();

        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(
            userId: userId,
            name: trainingName,
            exercises: new List<Exercise> { exercise }
        );

        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.ExistsByUserIdAndNameAsync(
                userId,
                trainingName,
                CancellationToken.None
            );

            // Assert
            result.Should().BeTrue();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task ExistsByUserIdAndNameAsync_WhenTrainingDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var userId = UserId.NewId();
        var trainingName = "Non-existent Training";

        try
        {
            // Act
            var result = await _sut.ExistsByUserIdAndNameAsync(
                userId,
                trainingName,
                CancellationToken.None
            );

            // Assert
            result.Should().BeFalse();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task ExistsByUserIdAndNameAsync_WhenTrainingExistsForDifferentUser_ShouldReturnFalse()
    {
        // Arrange
        var userId1 = UserId.NewId();
        var userId2 = UserId.NewId();
        var trainingName = "Test Training";
        var exercise = ExerciseFaker.Generate();

        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        var training = TrainingFaker.Generate(
            userId: userId1,
            name: trainingName,
            exercises: new List<Exercise> { exercise }
        );

        await WriteDbContext.Trainings.AddAsync(training);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.ExistsByUserIdAndNameAsync(
                userId2,
                trainingName,
                CancellationToken.None
            );

            // Assert
            result.Should().BeFalse();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenTrainingExists_ShouldReturnReadModelWithExercises()
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
