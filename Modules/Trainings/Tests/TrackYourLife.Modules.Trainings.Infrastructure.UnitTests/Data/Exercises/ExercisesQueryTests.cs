using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Infrastructure.UnitTests.Data.Exercises;

public class ExercisesQueryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private ExercisesQuery _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new ExercisesQuery(ReadDbContext);
    }

    [Fact]
    public async Task ExistsByUserIdAndNameAsync_WhenExerciseExists_ShouldReturnTrue()
    {
        // Arrange
        var userId = UserId.NewId();
        var exerciseName = "Test Exercise";
        var exercise = ExerciseFaker.Generate(userId: userId, name: exerciseName);

        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.ExistsByUserIdAndNameAsync(
                userId,
                exerciseName,
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
    public async Task ExistsByUserIdAndNameAsync_WhenExerciseDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var userId = UserId.NewId();
        var exerciseName = "Non-existent Exercise";

        try
        {
            // Act
            var result = await _sut.ExistsByUserIdAndNameAsync(
                userId,
                exerciseName,
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
    public async Task ExistsByUserIdAndNameAsync_WhenExerciseExistsForDifferentUser_ShouldReturnFalse()
    {
        // Arrange
        var userId1 = UserId.NewId();
        var userId2 = UserId.NewId();
        var exerciseName = "Test Exercise";
        var exercise = ExerciseFaker.Generate(userId: userId1, name: exerciseName);

        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.ExistsByUserIdAndNameAsync(
                userId2,
                exerciseName,
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
    public async Task GetByUserIdAndNameAsync_WhenExerciseExists_ShouldReturnReadModel()
    {
        // Arrange
        var userId = UserId.NewId();
        var exerciseName = "Test Exercise";
        var exercise = ExerciseFaker.Generate(userId: userId, name: exerciseName);

        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByUserIdAndNameAsync(
                userId,
                exerciseName,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(exercise.Id);
            result.UserId.Should().Be(userId);
            result.Name.Should().Be(exerciseName);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByUserIdAndNameAsync_WhenExerciseDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var userId = UserId.NewId();
        var exerciseName = "Non-existent Exercise";

        try
        {
            // Act
            var result = await _sut.GetByUserIdAndNameAsync(
                userId,
                exerciseName,
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
    public async Task GetEnumerableByUserIdAsync_WhenExercisesExist_ShouldReturnMatchingExercises()
    {
        // Arrange
        var userId = UserId.NewId();
        var exercise1 = ExerciseFaker.Generate(userId: userId);
        var exercise2 = ExerciseFaker.Generate(userId: userId);
        var exercise3 = ExerciseFaker.Generate(); // Different user

        await WriteDbContext.Exercises.AddAsync(exercise1);
        await WriteDbContext.Exercises.AddAsync(exercise2);
        await WriteDbContext.Exercises.AddAsync(exercise3);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetEnumerableByUserIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(e => e.Id == exercise1.Id);
            result.Should().Contain(e => e.Id == exercise2.Id);
            result.Should().NotContain(e => e.Id == exercise3.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetEnumerableByUserIdAsync_WhenNoExercisesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();

        try
        {
            // Act
            var result = await _sut.GetEnumerableByUserIdAsync(userId, CancellationToken.None);

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
    public async Task GetEnumerableByUserIdAsync_ShouldReturnExercisesOrderedByCreatedOnUtc()
    {
        // Arrange
        var userId = UserId.NewId();
        var now = DateTime.UtcNow;
        var exercise1 = ExerciseFaker.Generate(
            userId: userId,
            createdOnUtc: now.AddDays(-2)
        );
        var exercise2 = ExerciseFaker.Generate(
            userId: userId,
            createdOnUtc: now.AddDays(-1)
        );
        var exercise3 = ExerciseFaker.Generate(userId: userId, createdOnUtc: now);

        await WriteDbContext.Exercises.AddAsync(exercise1);
        await WriteDbContext.Exercises.AddAsync(exercise2);
        await WriteDbContext.Exercises.AddAsync(exercise3);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetEnumerableByUserIdAsync(userId, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().BeInAscendingOrder(e => e.CreatedOnUtc);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetEnumerableWithinIdsCollectionAsync_WhenExercisesExist_ShouldReturnMatchingExercises()
    {
        // Arrange
        var exercise1 = ExerciseFaker.Generate();
        var exercise2 = ExerciseFaker.Generate();
        var exercise3 = ExerciseFaker.Generate();

        await WriteDbContext.Exercises.AddAsync(exercise1);
        await WriteDbContext.Exercises.AddAsync(exercise2);
        await WriteDbContext.Exercises.AddAsync(exercise3);
        await WriteDbContext.SaveChangesAsync();

        var exerciseIds = new List<ExerciseId> { exercise1.Id, exercise3.Id };

        try
        {
            // Act
            var result = await _sut.GetEnumerableWithinIdsCollectionAsync(
                exerciseIds,
                CancellationToken.None
            );

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(e => e.Id == exercise1.Id);
            result.Should().Contain(e => e.Id == exercise3.Id);
            result.Should().NotContain(e => e.Id == exercise2.Id);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetEnumerableWithinIdsCollectionAsync_WhenNoMatchingExercisesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var exerciseIds = new List<ExerciseId> { ExerciseId.NewId(), ExerciseId.NewId() };

        try
        {
            // Act
            var result = await _sut.GetEnumerableWithinIdsCollectionAsync(
                exerciseIds,
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
}
