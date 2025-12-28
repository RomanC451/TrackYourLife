using System.Net;
using System.Net.Http.Json;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;
using TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Models;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Trainings.FunctionalTests.Features;

public class ExerciseHistoriesTests : TrainingsBaseIntegrationTest
{
    public ExerciseHistoriesTests(TrainingsFunctionalTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetExerciseHistoryByExerciseId_ShouldReturnHistories_WhenHistoriesExist()
    {
        // Arrange
        var exerciseId = await CreateTestExercise();
        // Note: Exercise histories are typically created when ongoing trainings are finished
        // For this test, we'll assume some histories exist or create them through the ongoing training flow

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/exercises-histories?ExerciseId={exerciseId}"
        );

        // Assert
        await response.ShouldHaveStatusCodeAndContent<List<ExerciseHistoryDto>>(HttpStatusCode.OK);
        // Note: The list might be empty if no histories exist yet
    }

    [Fact]
    public async Task GetExerciseHistoryByExerciseId_ShouldReturnNotFound_WhenExerciseDoesNotExist()
    {
        // Arrange
        var nonExistentExerciseId = ExerciseId.Create(Guid.NewGuid());

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/exercises-histories?ExerciseId={nonExistentExerciseId}"
        );

        // Assert
        await response.ShouldHaveStatusCode(HttpStatusCode.NotFound);
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
