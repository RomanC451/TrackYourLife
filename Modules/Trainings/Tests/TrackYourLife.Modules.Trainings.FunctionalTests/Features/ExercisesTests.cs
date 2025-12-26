using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Trainings.FunctionalTests.Features;

public class ExercisesTests : TrainingsBaseIntegrationTest
{
    public ExercisesTests(TrainingsFunctionalTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateExercise_ShouldReturnCreated_WhenValidData()
    {
        // Arrange
        var command = new CreateExerciseRequest(
            Name: "Test Exercise",
            MuscleGroups: new List<string> { "Chest", "Triceps" },
            Difficulty: Difficulty.Medium,
            Description: "Test exercise description",
            PictureUrl: null,
            VideoUrl: null,
            Equipment: null,
            ExerciseSets: new List<ExerciseSetData>
            {
                new ExerciseSetData
                {
                    Type = ExerciseSetType.Weight,
                    Name = "Set 1",
                    OrderIndex = 0,
                    Reps = 10,
                    Weight = 50.0f,
                },
                new ExerciseSetData
                {
                    Type = ExerciseSetType.Weight,
                    Name = "Set 2",
                    OrderIndex = 1,
                    Reps = 8,
                    Weight = 60.0f,
                },
            }
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/exercises", command);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify exercise was created in database
        var location = response.Headers.Location?.ToString();
        if (string.IsNullOrEmpty(location))
        {
            throw new InvalidOperationException("Location header is missing from the response");
        }

        var locationParts = location.Split('/');
        var exerciseIdString = locationParts[locationParts.Length - 1];
        var exerciseId = Guid.Parse(exerciseIdString);

        var exercise = await _trainingsWriteDbContext.Exercises.FirstAsync(e =>
            e.Id == ExerciseId.Create(exerciseId)
        );

        exercise.Should().NotBeNull();
        exercise.Name.Should().Be("Test Exercise");
        exercise.MuscleGroups.Should().BeEquivalentTo("Chest", "Triceps");
        exercise.Difficulty.Should().Be(Difficulty.Medium);
        exercise.Description.Should().Be("Test exercise description");
        exercise.UserId.Should().Be(_user.Id);
        exercise.ExerciseSets.Should().HaveCount(2);
        exercise.ExerciseSets[0].Name.Should().Be("Set 1");
        ((WeightBasedExerciseSet)exercise.ExerciseSets[0]).Reps.Should().Be(10);
        ((WeightBasedExerciseSet)exercise.ExerciseSets[0]).Weight.Should().Be(50.0f);
        exercise.ExerciseSets[0].OrderIndex.Should().Be(0);
        exercise.ExerciseSets[1].Name.Should().Be("Set 2");
        ((WeightBasedExerciseSet)exercise.ExerciseSets[1]).Reps.Should().Be(8);
        ((WeightBasedExerciseSet)exercise.ExerciseSets[1]).Weight.Should().Be(60.0f);
        exercise.ExerciseSets[1].OrderIndex.Should().Be(1);
    }

    [Fact]
    public async Task GetExercises_ShouldReturnExercises_WhenExercisesExist()
    {
        // Arrange
        await CreateTestExercise();

        // Act
        var response = await HttpClient.GetAsync("/api/exercises");

        // Assert
        var exercises = await response.ShouldHaveStatusCodeAndContent<List<ExerciseDto>>(
            HttpStatusCode.OK
        );
        exercises.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetExerciseById_ShouldReturnExercise_WhenExerciseExists()
    {
        // Arrange
        var exerciseId = await CreateTestExercise();

        // Act
        var response = await HttpClient.GetAsync($"/api/exercises/{exerciseId}");

        // Assert
        var exercise = await response.ShouldHaveStatusCodeAndContent<ExerciseDto>(
            HttpStatusCode.OK
        );
        exercise.Id.Should().Be(exerciseId);
    }

    [Fact]
    public async Task UpdateExercise_ShouldReturnNoContent_WhenValidData()
    {
        // Arrange
        var exerciseId = await CreateTestExercise();
        var updateCommand = new UpdateExerciseRequest(
            Name: "Updated Exercise",
            MuscleGroups: new List<string> { "Chest", "Shoulders" },
            Difficulty: Difficulty.Hard,
            Description: "Updated description",
            VideoUrl: null,
            PictureUrl: null,
            Equipment: null,
            ExerciseSets: new List<ExerciseSet>
            {
                new WeightBasedExerciseSet(Guid.NewGuid(), "Set 1", 0, 12, 55.0f),
            }
        );

        // Act
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        var json = JsonSerializer.Serialize(updateCommand, jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await HttpClient.PutAsync($"/api/exercises/{exerciseId}", content);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify exercise was updated in database
        var updatedExercise = await _trainingsWriteDbContext.Exercises.FirstAsync(e =>
            e.Id == exerciseId
        );

        updatedExercise.Should().NotBeNull();
        updatedExercise.Name.Should().Be("Updated Exercise");
        updatedExercise.MuscleGroups.Should().BeEquivalentTo("Chest", "Shoulders");
        updatedExercise.Difficulty.Should().Be(Difficulty.Hard);
        updatedExercise.Description.Should().Be("Updated description");
        updatedExercise.UserId.Should().Be(_user.Id);
        updatedExercise.ExerciseSets.Should().HaveCount(1);
        updatedExercise.ExerciseSets[0].Name.Should().Be("Set 1");
        ((WeightBasedExerciseSet)updatedExercise.ExerciseSets[0]).Reps.Should().Be(12);
        ((WeightBasedExerciseSet)updatedExercise.ExerciseSets[0]).Weight.Should().Be(55.0f);
        updatedExercise.ExerciseSets[0].OrderIndex.Should().Be(0);
    }

    [Fact]
    public async Task DeleteExercise_ShouldReturnNoContent_WhenExerciseExists()
    {
        // Arrange
        var exerciseId = await CreateTestExercise();

        // Verify exercise exists before deletion
        var exerciseBeforeDelete = await _trainingsWriteDbContext.Exercises.FirstAsync(e =>
            e.Id == exerciseId
        );
        exerciseBeforeDelete.Should().NotBeNull();

        // Act
        var response = await HttpClient.DeleteAsync($"/api/exercises/{exerciseId}");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify exercise was removed from database
        _trainingsWriteDbContext.ChangeTracker.Clear();
        var exerciseAfterDelete = await _trainingsWriteDbContext.Exercises.FirstOrDefaultAsync(e =>
            e.Id == exerciseId
        );

        exerciseAfterDelete.Should().BeNull();
    }

    private async Task<ExerciseId> CreateTestExercise()
    {
        var command = new CreateExerciseRequest(
            Name: $"Test Exercise {Guid.NewGuid()}",
            MuscleGroups: new List<string> { "Chest" },
            Difficulty: Difficulty.Medium,
            Description: "Test exercise",
            PictureUrl: null,
            VideoUrl: null,
            Equipment: null,
            ExerciseSets: new List<ExerciseSetData>
            {
                new ExerciseSetData
                {
                    Type = ExerciseSetType.Weight,
                    Name = "Set 1",
                    OrderIndex = 0,
                    Reps = 10,
                    Weight = 50.0f,
                },
            }
        );

        var response = await HttpClient.PostAsJsonAsync("/api/exercises", command);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString();
        if (string.IsNullOrEmpty(location))
        {
            throw new InvalidOperationException("Location header is missing from the response");
        }

        var locationParts = location.Split('/');
        var exerciseIdString = locationParts[locationParts.Length - 1];

        if (
            string.IsNullOrEmpty(exerciseIdString)
            || !Guid.TryParse(exerciseIdString, out var exerciseId)
        )
        {
            throw new InvalidOperationException(
                $"Invalid exercise ID in location header: {location}"
            );
        }

        return ExerciseId.Create(exerciseId);
    }
}
