using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Commands;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings;
using TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Commands;
using TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans;
using TrackYourLife.Modules.Trainings.Presentation.Features.WorkoutPlans.Commands;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Trainings.FunctionalTests.Features;

public class WorkoutPlansTests : TrainingsBaseIntegrationTest
{
    public WorkoutPlansTests(TrainingsFunctionalTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetWorkoutPlans_ShouldReturn200_WithEmptyList_WhenNoPlans()
    {
        var response = await HttpClient.GetAsync("/api/workout-plans");

        var plans = await response.ShouldHaveStatusCodeAndContent<List<WorkoutPlanDto>>(
            HttpStatusCode.OK
        );
        plans.Should().NotBeNull();
        plans.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateWorkoutPlan_ThenGetWorkoutPlans_ShouldContainPlan()
    {
        var trainingId = await CreateTestTraining();

        var createResponse = await HttpClient.PostAsJsonAsync(
            "/api/workout-plans",
            new CreateWorkoutPlanRequest("Weekly", false, [trainingId])
        );

        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        await WaitForOutboxEventsToBeHandledAsync();

        var listResponse = await HttpClient.GetAsync("/api/workout-plans");
        var plans = await listResponse.ShouldHaveStatusCodeAndContent<List<WorkoutPlanDto>>(
            HttpStatusCode.OK
        );

        plans.Should().ContainSingle(p => p.Name == "Weekly");
    }

    [Fact]
    public async Task GetActiveWorkoutPlan_ShouldReturn200_WhenPlanExists()
    {
        var trainingId = await CreateTestTraining();
        await HttpClient.PostAsJsonAsync(
            "/api/workout-plans",
            new CreateWorkoutPlanRequest("Active plan", true, [trainingId])
        );
        await WaitForOutboxEventsToBeHandledAsync();

        var response = await HttpClient.GetAsync("/api/workout-plans/active");

        var plan = await response.ShouldHaveStatusCodeAndContent<WorkoutPlanDto>(HttpStatusCode.OK);
        plan.Name.Should().Be("Active plan");
        plan.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetNextWorkoutFromActivePlan_ShouldReturn200_WhenPlanExists()
    {
        var trainingId = await CreateTestTraining();
        await HttpClient.PostAsJsonAsync(
            "/api/workout-plans",
            new CreateWorkoutPlanRequest("Rotation", true, [trainingId])
        );
        await WaitForOutboxEventsToBeHandledAsync();

        var response = await HttpClient.GetAsync("/api/workout-plans/next-workout");

        var training = await response.ShouldHaveStatusCodeAndContent<TrainingDto>(HttpStatusCode.OK);
        training.Id.Should().Be(trainingId);
    }

    [Fact]
    public async Task DeleteWorkoutPlan_ShouldReturn204_WhenPlanExists()
    {
        var trainingId = await CreateTestTraining();
        var createResponse = await HttpClient.PostAsJsonAsync(
            "/api/workout-plans",
            new CreateWorkoutPlanRequest("To delete", true, [trainingId])
        );
        await createResponse.ShouldHaveStatusCode(HttpStatusCode.Created);
        await WaitForOutboxEventsToBeHandledAsync();

        var planId = await _trainingsWriteDbContext
            .WorkoutPlans.AsNoTracking()
            .Where(p => p.Name == "To delete")
            .Select(p => p.Id)
            .FirstAsync();

        var deleteResponse = await HttpClient.DeleteAsync($"/api/workout-plans/{planId.Value}");
        await deleteResponse.ShouldHaveStatusCode(HttpStatusCode.NoContent);
        await WaitForOutboxEventsToBeHandledAsync();

        var stillThere = await _trainingsWriteDbContext.WorkoutPlans.AnyAsync(p => p.Id == planId);
        stillThere.Should().BeFalse();
    }

    private async Task<TrainingId> CreateTestTraining()
    {
        var exerciseId = await CreateTestExercise();
        var command = new CreateTrainingRequest(
            Name: "Workout plan training",
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
        var trainingIdString = locationParts[^1];

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
            Name: $"Plan test exercise {Guid.NewGuid()}",
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
        var exerciseIdString = locationParts[^1];

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
