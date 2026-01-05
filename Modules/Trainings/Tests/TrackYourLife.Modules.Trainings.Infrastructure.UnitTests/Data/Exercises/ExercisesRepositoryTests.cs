using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Infrastructure.Data;
using TrackYourLife.Modules.Trainings.Infrastructure.Data.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Trainings.Infrastructure.UnitTests.Data.Exercises;

public class ExercisesRepositoryTests(DatabaseFixture fixture) : BaseRepositoryTests(fixture)
{
    private ExercisesRepository _sut = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = new ExercisesRepository(WriteDbContext);
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

    [Fact]
    public async Task GetEnumerableWithinIdsCollectionAsync_WithEmptyIdsList_ShouldReturnEmptyList()
    {
        // Arrange
        var exerciseIds = new List<ExerciseId>();

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

    [Fact]
    public async Task AddAsync_WhenExerciseIsAdded_ShouldPersistToDatabase()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();

        try
        {
            // Act
            await _sut.AddAsync(exercise, CancellationToken.None);
            await WriteDbContext.SaveChangesAsync();

            // Assert
            var result = await WriteDbContext.Exercises.FindAsync(exercise.Id);
            result.Should().NotBeNull();
            result!.Id.Should().Be(exercise.Id);
            result.Name.Should().Be(exercise.Name);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenExerciseExists_ShouldReturnExercise()
    {
        // Arrange
        var exercise = ExerciseFaker.Generate();
        await WriteDbContext.Exercises.AddAsync(exercise);
        await WriteDbContext.SaveChangesAsync();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(exercise.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(exercise.Id);
            result.Name.Should().Be(exercise.Name);
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }

    [Fact]
    public async Task GetByIdAsync_WhenExerciseDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var exerciseId = ExerciseId.NewId();

        try
        {
            // Act
            var result = await _sut.GetByIdAsync(exerciseId, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
        finally
        {
            await CleanupAllDbSets();
        }
    }
}
