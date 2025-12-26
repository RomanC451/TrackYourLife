using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Trainings.FunctionalTests.Features;

public class TrainingsTests : TrainingsBaseIntegrationTest
{
    public TrainingsTests(TrainingsFunctionalTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateTraining_ShouldReturnCreated_WhenValidData()
    {
        // Arrange
        var exerciseId = await CreateTestExercise();
        var command = new CreateTrainingRequest(
            Name: "Test Training",
            MuscleGroups: new List<string> { "Chest", "Triceps" },
            Difficulty: Difficulty.Medium,
            ExercisesIds: new List<ExerciseId> { exerciseId },
            Description: "Test training description",
            Duration: 45,
            RestSeconds: 90
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/trainings", command);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify training was created in database
        var training = await _trainingsWriteDbContext
            .Trainings.Include(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstOrDefaultAsync(t => t.Name == "Test Training");

        training.Should().NotBeNull();
        training!.Name.Should().Be("Test Training");
        training.MuscleGroups.Should().Contain("Chest");
        training.MuscleGroups.Should().Contain("Triceps");
        training.Difficulty.Should().Be(Difficulty.Medium);
        training.Duration.Should().Be(45);
        training.RestSeconds.Should().Be(90);
        training.Description.Should().Be("Test training description");
        training.TrainingExercises.Should().HaveCount(1);
        training.TrainingExercises.First().Exercise.Id.Should().Be(exerciseId);
    }

    [Fact]
    public async Task GetTrainings_ShouldReturnTrainings_WhenTrainingsExist()
    {
        // Arrange
        await CreateTestTraining();

        // Act
        var response = await HttpClient.GetAsync("/api/trainings");

        // Assert
        var trainings = await response.ShouldHaveStatusCodeAndContent<List<TrainingDto>>(
            HttpStatusCode.OK
        );
        trainings.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task UpdateTraining_ShouldReturnNoContent_WhenValidData()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        var exerciseId = await CreateTestExercise();
        var updateCommand = new UpdateTrainingRequest(
            Name: "Updated Training",
            MuscleGroups: new List<string> { "Chest", "Shoulders" },
            Difficulty: Difficulty.Hard,
            Duration: 60,
            RestSeconds: 120,
            Description: "Updated training description",
            ExercisesIds: new List<ExerciseId> { exerciseId }
        );

        // Verify initial state
        var initialTraining = await _trainingsWriteDbContext.Trainings.FirstOrDefaultAsync(t =>
            t.Id == trainingId
        );
        initialTraining!.Name.Should().Be("Test Training");

        // Act
        var response = await HttpClient.PutAsJsonAsync(
            $"/api/trainings/{trainingId}",
            updateCommand
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify training was updated in database
        _trainingsWriteDbContext.ChangeTracker.Clear();
        var updatedTraining = await _trainingsWriteDbContext
            .Trainings.Include(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstOrDefaultAsync(t => t.Id == trainingId);

        updatedTraining.Should().NotBeNull();
        updatedTraining!.Name.Should().Be("Updated Training");
        updatedTraining.MuscleGroups.Should().Contain("Chest");
        updatedTraining.MuscleGroups.Should().Contain("Shoulders");
        updatedTraining.Difficulty.Should().Be(Difficulty.Hard);
        updatedTraining.Duration.Should().Be(60);
        updatedTraining.RestSeconds.Should().Be(120);
        updatedTraining.Description.Should().Be("Updated training description");
        updatedTraining.TrainingExercises.Should().HaveCount(1);
        updatedTraining.TrainingExercises.First().Exercise.Id.Should().Be(exerciseId);
    }

    [Fact]
    public async Task DeleteTraining_ShouldReturnNoContent_WhenTrainingExists()
    {
        // Arrange
        var trainingId = await CreateTestTraining();

        // Verify training exists before deletion
        var trainingBeforeDelete = await _trainingsWriteDbContext.Trainings.FirstOrDefaultAsync(t =>
            t.Id == trainingId
        );
        trainingBeforeDelete.Should().NotBeNull();

        // Act
        var response = await HttpClient.DeleteAsync($"/api/trainings/{trainingId}");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify training was removed from database
        var trainingAfterDelete = await _trainingsWriteDbContext.Trainings.FirstOrDefaultAsync(t =>
            t.Id == trainingId
        );

        trainingAfterDelete.Should().BeNull();

        // Verify related training exercises were also removed
        var trainingExercises = await _trainingsWriteDbContext
            .TrainingExercises.Where(te => te.TrainingId == trainingId)
            .ToListAsync();

        trainingExercises.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateTraining_ShouldReturnBadRequest_WhenExerciseDoesNotExist()
    {
        // Arrange
        var nonExistentExerciseId = Guid.NewGuid();
        var command = new CreateTrainingRequest(
            Name: "Test Training",
            MuscleGroups: new List<string> { "Chest" },
            Difficulty: Difficulty.Medium,
            ExercisesIds: new List<ExerciseId> { ExerciseId.Create(nonExistentExerciseId) },
            Description: "Test training",
            Duration: 45,
            RestSeconds: 90
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/trainings", command);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.BadRequest);
    }

    private async Task<TrainingId> CreateTestTraining()
    {
        var exerciseId = await CreateTestExercise();
        var command = new CreateTrainingRequest(
            Name: "Test Training",
            MuscleGroups: new List<string> { "Chest" },
            Difficulty: Difficulty.Medium,
            ExercisesIds: new List<ExerciseId> { exerciseId },
            Description: "Test training",
            Duration: 45,
            RestSeconds: 90
        );

        var response = await HttpClient.PostAsJsonAsync("/api/trainings", command);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString();
        if (string.IsNullOrEmpty(location))
        {
            throw new InvalidOperationException("Location header is missing from the response");
        }

        var locationParts = location.Split('/');
        var trainingIdString = locationParts[locationParts.Length - 1];

        if (
            string.IsNullOrEmpty(trainingIdString)
            || !Guid.TryParse(trainingIdString, out var trainingId)
        )
        {
            throw new InvalidOperationException(
                $"Invalid training ID in location header: {location}"
            );
        }

        return TrainingId.Create(trainingId);
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
            ExerciseSets: new List<ExerciseSetDto>
            {
                new WeightBasedExerciseSetDto(
                    Id: null,
                    Name: "Set 1",
                    OrderIndex: 0,
                    Reps: 10,
                    Weight: 50.0f
                ),
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
