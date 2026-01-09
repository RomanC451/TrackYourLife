using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.OngoingTrainings.Commands;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Trainings.FunctionalTests.Features;

public class OngoingTrainingsTests : TrainingsBaseIntegrationTest
{
    public OngoingTrainingsTests(TrainingsFunctionalTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task CreateOngoingTraining_ShouldReturnCreated_WhenValidData()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        var command = new CreateOngoingTrainingRequest(trainingId);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/ongoing-trainings", command);

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.Created);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify ongoing training was created in database
        var ongoingTraining = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .FirstAsync(ot => ot.Training.Id == trainingId);

        ongoingTraining.Should().NotBeNull();
        ongoingTraining.UserId.Should().Be(_user.Id);
        ongoingTraining.Training.Id.Should().Be(trainingId);
        ongoingTraining.ExerciseIndex.Should().Be(0);
        ongoingTraining.SetIndex.Should().Be(0);
        ongoingTraining.FinishedOnUtc.Should().BeNull();
        ongoingTraining.StartedOnUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetActiveOngoingTraining_ShouldReturnOngoingTraining_WhenActiveTrainingExists()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        await CreateOngoingTraining(trainingId);

        // Act
        var response = await HttpClient.GetAsync("/api/ongoing-trainings/active-training");

        // Assert
        var ongoingTraining = await response.ShouldHaveStatusCodeAndContent<OngoingTrainingDto>(
            HttpStatusCode.OK
        );
        ongoingTraining.Training.Id.Should().Be(trainingId);
    }

    [Fact]
    public async Task GetOngoingTrainingById_ShouldReturnOngoingTraining_WhenOngoingTrainingExists()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        var ongoingTrainingId = await CreateOngoingTraining(trainingId);

        // Act
        var response = await HttpClient.GetAsync($"/api/ongoing-trainings/{ongoingTrainingId}");

        // Assert
        var ongoingTraining = await response.ShouldHaveStatusCodeAndContent<OngoingTrainingDto>(
            HttpStatusCode.OK
        );
        ongoingTraining.Id.Should().Be(ongoingTrainingId);
    }

    [Fact]
    public async Task NextOngoingTraining_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        var ongoingTrainingId = await CreateOngoingTraining(trainingId);

        // Verify initial state
        var initialOngoingTraining = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);
        initialOngoingTraining.ExerciseIndex.Should().Be(0);
        initialOngoingTraining.SetIndex.Should().Be(0);

        // Act
        var response = await HttpClient.PutAsync(
            $"/api/ongoing-trainings/active-training/next",
            JsonContent.Create(new NextOngoingTrainingRequest(ongoingTrainingId))
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify state was updated in database
        // Detach any tracked entities to ensure we get fresh data from database
        _trainingsWriteDbContext.ChangeTracker.Clear();

        var updatedOngoingTraining = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);

        updatedOngoingTraining.Should().NotBeNull();
        updatedOngoingTraining.ExerciseIndex.Should().Be(0); // Should still be 0 for first exercise
        updatedOngoingTraining.SetIndex.Should().Be(1); // Should move to next set
    }

    [Fact]
    public async Task PreviousOngoingTraining_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        var ongoingTrainingId = await CreateOngoingTraining(trainingId);

        // First move to next to have a previous position
        await HttpClient.PutAsync(
            $"/api/ongoing-trainings/active-training/next",
            JsonContent.Create(new NextOngoingTrainingRequest(ongoingTrainingId))
        );

        // Verify we're at set index 1
        var ongoingTrainingAfterNext = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);
        ongoingTrainingAfterNext.SetIndex.Should().Be(1);

        // Act
        var response = await HttpClient.PutAsync(
            $"/api/ongoing-trainings/active-training/previous",
            JsonContent.Create(new PreviousOngoingTrainingRequest(ongoingTrainingId))
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify state was reverted in database
        // Detach any tracked entities to ensure we get fresh data from database
        _trainingsWriteDbContext.ChangeTracker.Clear();

        var ongoingTrainingAfterPrevious = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);

        ongoingTrainingAfterPrevious.Should().NotBeNull();
        ongoingTrainingAfterPrevious.ExerciseIndex.Should().Be(0);
        ongoingTrainingAfterPrevious.SetIndex.Should().Be(0); // Should be back to first set
    }

    [Fact]
    public async Task AdjustExerciseSets_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        var ongoingTrainingId = await CreateOngoingTraining(trainingId);

        // Get the ongoing training to access exercise information
        var ongoingTrainingResponse = await HttpClient.GetAsync(
            "/api/ongoing-trainings/active-training"
        );
        var ongoingTraining =
            await ongoingTrainingResponse.ShouldHaveStatusCodeAndContent<OngoingTrainingDto>(
                HttpStatusCode.OK
            );

        // Get the first exercise and its first set
        var exerciseId = ongoingTraining.Training.Exercises.First().Id;
        var originalSet = ongoingTraining.Training.Exercises.First().ExerciseSets.First();

        // Create new exercise set with updated values
        var newExerciseSet = ExerciseSet
            .Create(
                originalSet.Id,
                originalSet.Name,
                originalSet.OrderIndex,
                originalSet.Count1 + 2, // Apply Count1Change
                originalSet.Unit1, // Keep original unit if Unit1Change is null
                (originalSet.Count2 ?? 0) + 5.0f, // Apply Count2Change
                originalSet.Unit2 ?? "kg" // Keep original unit if Unit2Change is null
            )
            .Value;

        var command = new AdjustExerciseSetsRequest(
            exerciseId,
            new List<ExerciseSet> { newExerciseSet }
        );

        // Act
        var response = await HttpClient.PutAsJsonAsync(
            $"/api/ongoing-trainings/{ongoingTrainingId}/adjust-sets",
            command
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify exercise history was created in database
        var exerciseHistory = await _trainingsWriteDbContext.ExerciseHistories.FirstAsync(eh =>
            eh.OngoingTrainingId == ongoingTrainingId && eh.ExerciseId == exerciseId
        );

        exerciseHistory.Should().NotBeNull();
        exerciseHistory.OngoingTrainingId.Should().Be(ongoingTrainingId);
        exerciseHistory.ExerciseId.Should().Be(exerciseId);
        exerciseHistory.AreChangesApplied.Should().BeFalse(); // Changes should not be applied yet
        exerciseHistory.NewExerciseSets.Should().HaveCount(1);
        exerciseHistory.NewExerciseSets[0].Id.Should().Be(originalSet.Id);
        // Verify the new exercise set has the updated values
        exerciseHistory.NewExerciseSets[0].Count1.Should().BeGreaterThan(0);
        exerciseHistory.NewExerciseSets[0].Count2.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task FinishOngoingTraining_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        var ongoingTrainingId = await CreateOngoingTraining(trainingId);

        // Verify initial state
        var initialOngoingTraining = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);
        initialOngoingTraining.FinishedOnUtc.Should().BeNull();

        // Get the ongoing training to access exercise information
        var ongoingTrainingResponse = await HttpClient.GetAsync(
            "/api/ongoing-trainings/active-training"
        );
        var ongoingTraining =
            await ongoingTrainingResponse.ShouldHaveStatusCodeAndContent<OngoingTrainingDto>(
                HttpStatusCode.OK
            );

        // Mark all exercises as completed by adjusting their sets
        // This is required before finishing a training
        foreach (var exercise in ongoingTraining.Training.Exercises)
        {
            var exerciseId = exercise.Id;

            // Create new exercise sets with same values for all sets (completing the exercise)
            var newExerciseSets = exercise
                .ExerciseSets.Select(originalSet =>
                    ExerciseSet
                        .Create(
                            originalSet.Id,
                            originalSet.Name,
                            originalSet.OrderIndex,
                            originalSet.Count1,
                            originalSet.Unit1,
                            originalSet.Count2 ?? 0,
                            originalSet.Unit2 ?? "kg"
                        )
                        .Value
                )
                .ToList();

            var adjustCommand = new AdjustExerciseSetsRequest(exerciseId, newExerciseSets);

            var adjustResponse = await HttpClient.PutAsJsonAsync(
                $"/api/ongoing-trainings/{ongoingTrainingId}/adjust-sets",
                adjustCommand
            );
            adjustResponse.EnsureSuccessStatusCode();
        }

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Act
        var response = await HttpClient.PostAsync(
            $"/api/ongoing-trainings/{ongoingTrainingId}/finish",
            null
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify ongoing training was marked as finished in database
        _trainingsWriteDbContext.ChangeTracker.Clear();
        var finishedOngoingTraining = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);

        finishedOngoingTraining.Should().NotBeNull();
        finishedOngoingTraining.FinishedOnUtc.Should().NotBeNull();
        finishedOngoingTraining
            .FinishedOnUtc.Should()
            .BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        finishedOngoingTraining.IsFinished.Should().BeTrue();
    }

    [Fact]
    public async Task CancelOngoingTraining_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var trainingId = await CreateTestTraining();
        var ongoingTrainingId = await CreateOngoingTraining(trainingId);

        // Verify ongoing training exists before cancellation
        var ongoingTrainingBeforeCancel = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);
        ongoingTrainingBeforeCancel.Should().NotBeNull();

        // Act
        var response = await HttpClient.DeleteAsync($"/api/ongoing-trainings/{trainingId}");

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify ongoing training was removed from database
        _trainingsWriteDbContext.ChangeTracker.Clear();
        var ongoingTrainingAfterCancel = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstOrDefaultAsync(ot => ot.Id == ongoingTrainingId);

        ongoingTrainingAfterCancel.Should().BeNull();
    }

    [Fact]
    public async Task SkipExercise_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var trainingId = await CreateTestTrainingWithMultipleExercises();
        var ongoingTrainingId = await CreateOngoingTraining(trainingId);

        // Get the ongoing training to verify initial state
        var ongoingTrainingResponse = await HttpClient.GetAsync(
            "/api/ongoing-trainings/active-training"
        );
        var ongoingTraining =
            await ongoingTrainingResponse.ShouldHaveStatusCodeAndContent<OngoingTrainingDto>(
                HttpStatusCode.OK
            );

        var initialExerciseIndex = ongoingTraining.ExerciseIndex;
        var currentExerciseId = ongoingTraining
            .Training.Exercises.ElementAt(initialExerciseIndex)
            .Id;

        // Act
        var response = await HttpClient.PutAsync(
            "/api/ongoing-trainings/active-training/skip",
            JsonContent.Create(new SkipExerciseRequest(ongoingTrainingId))
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify exercise history was created with Skipped status
        _trainingsWriteDbContext.ChangeTracker.Clear();
        var exerciseHistory = await _trainingsWriteDbContext.ExerciseHistories.FirstAsync(eh =>
            eh.OngoingTrainingId == ongoingTrainingId && eh.ExerciseId == currentExerciseId
        );

        exerciseHistory.Should().NotBeNull();
        exerciseHistory.Status.Should().Be(ExerciseStatus.Skipped);
        exerciseHistory.NewExerciseSets.Should().BeEmpty();

        // Verify ongoing training moved to next incomplete exercise
        _trainingsWriteDbContext.ChangeTracker.Clear();
        var updatedOngoingTraining = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);

        updatedOngoingTraining.ExerciseIndex.Should().NotBe(initialExerciseIndex);
        updatedOngoingTraining.SetIndex.Should().Be(0); // Should reset to first set
    }

    [Fact]
    public async Task JumpToExercise_ShouldReturnNoContent_WhenValidRequest()
    {
        // Arrange
        var trainingId = await CreateTestTrainingWithMultipleExercises();
        var ongoingTrainingId = await CreateOngoingTraining(trainingId);

        var targetExerciseIndex = 1; // Jump to second exercise

        // Act
        var response = await HttpClient.PutAsync(
            "/api/ongoing-trainings/active-training/jump-to-exercise",
            JsonContent.Create(new JumpToExerciseRequest(ongoingTrainingId, targetExerciseIndex))
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NoContent);

        // Wait for outbox events to be processed
        await WaitForOutboxEventsToBeHandledAsync();

        // Verify ongoing training moved to target exercise
        _trainingsWriteDbContext.ChangeTracker.Clear();
        var updatedOngoingTraining = await _trainingsWriteDbContext
            .OngoingTrainings.Include(ot => ot.Training)
            .ThenInclude(t => t.TrainingExercises)
            .ThenInclude(te => te.Exercise)
            .FirstAsync(ot => ot.Id == ongoingTrainingId);

        updatedOngoingTraining.Should().NotBeNull();
        updatedOngoingTraining.ExerciseIndex.Should().Be(targetExerciseIndex);
        updatedOngoingTraining.SetIndex.Should().Be(0); // Should reset to first set
    }

    private async Task<OngoingTrainingId> CreateOngoingTraining(TrainingId trainingId)
    {
        var command = new CreateOngoingTrainingRequest(trainingId);

        var response = await HttpClient.PostAsJsonAsync("/api/ongoing-trainings", command);
        response.EnsureSuccessStatusCode();

        var location = response.Headers.Location?.ToString();
        if (string.IsNullOrEmpty(location))
        {
            throw new InvalidOperationException("Location header is missing from the response");
        }

        var locationParts = location.Split('/');
        var ongoingTrainingIdString = locationParts[locationParts.Length - 1];

        if (
            string.IsNullOrEmpty(ongoingTrainingIdString)
            || !Guid.TryParse(ongoingTrainingIdString, out var ongoingTrainingId)
        )
        {
            throw new InvalidOperationException(
                $"Invalid ongoing training ID in location header: {location}"
            );
        }

        return OngoingTrainingId.Create(ongoingTrainingId);
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
            ExerciseSets: new List<ExerciseSet>
            {
                ExerciseSet.Create(Guid.NewGuid(), "Set 1", 0, 10, "reps", 50.0f, "kg").Value,
                ExerciseSet.Create(Guid.NewGuid(), "Set 2", 1, 8, "reps", 60.0f, "kg").Value,
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

    private async Task<TrainingId> CreateTestTrainingWithMultipleExercises()
    {
        var exerciseId1 = await CreateTestExercise();
        var exerciseId2 = await CreateTestExercise();
        var command = new CreateTrainingRequest(
            Name: "Test Training",
            MuscleGroups: new List<string> { "Chest", "Back" },
            Difficulty: Difficulty.Medium,
            ExercisesIds: new List<ExerciseId> { exerciseId1, exerciseId2 },
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
}
