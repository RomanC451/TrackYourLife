using System.Net;
using System.Net.Http.Json;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;
using TrackYourLife.SharedLib.FunctionalTests.Utils;

namespace TrackYourLife.Modules.Trainings.FunctionalTests.Features;

public class TrainingsOverviewFunctionalTests : TrainingsBaseIntegrationTest
{
    public TrainingsOverviewFunctionalTests(TrainingsFunctionalTestWebAppFactory factory)
        : base(factory) { }

    [Fact]
    public async Task GetTrainingsOverview_ShouldReturn200_WhenNoWorkouts()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/trainings/overview");

        // Assert
        var overview = await response.ShouldHaveStatusCodeAndContent<TrainingsOverviewDto>(
            HttpStatusCode.OK
        );
        overview.TotalWorkoutsCompleted.Should().Be(0);
        overview.TotalTrainingTimeSeconds.Should().Be(0);
        overview.TotalCaloriesBurned.Should().BeNull();
        overview.HasActiveTraining.Should().BeFalse();
    }

    [Fact]
    public async Task GetTrainingsOverview_WithQueryParams_ShouldReturn200()
    {
        // Act
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var response = await HttpClient.GetAsync(
            $"/api/trainings/overview?StartDate={startDate:yyyy-MM-dd}&EndDate={endDate:yyyy-MM-dd}"
        );

        // Assert
        var overview = await response.ShouldHaveStatusCodeAndContent<TrainingsOverviewDto>(
            HttpStatusCode.OK
        );
        overview.TotalWorkoutsCompleted.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetTrainingTemplatesUsage_ShouldReturn200_WhenNoWorkouts()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/trainings/templates-usage");

        // Assert
        var templates = await response.ShouldHaveStatusCodeAndContent<
            List<TrainingTemplateUsageDto>
        >(HttpStatusCode.OK);
        templates.Should().NotBeNull();
        templates.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDifficultyDistribution_ShouldReturn200_WhenCalled()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/trainings/difficulty-distribution");

        // Assert
        var distribution = await response.ShouldHaveStatusCodeAndContent<
            List<DifficultyDistributionDto>
        >(HttpStatusCode.OK);
        distribution.Should().NotBeNull();
    }

    [Fact]
    public async Task GetMuscleGroupDistribution_ShouldReturn200_WhenCalled()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/trainings/muscle-group-distribution");

        // Assert
        var distribution = await response.ShouldHaveStatusCodeAndContent<
            List<MuscleGroupDistributionDto>
        >(HttpStatusCode.OK);
        distribution.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkoutFrequency_ShouldReturn200_WhenCalled()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/trainings/frequency");

        // Assert
        var frequency = await response.ShouldHaveStatusCodeAndContent<
            List<WorkoutFrequencyDataDto>
        >(HttpStatusCode.OK);
        frequency.Should().NotBeNull();
    }

    [Fact]
    public async Task GetWorkoutDurationHistory_ShouldReturn200_WhenCalled()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/trainings/duration");

        // Assert
        var history = await response.ShouldHaveStatusCodeAndContent<
            List<WorkoutAggregatedValueDto>
        >(HttpStatusCode.OK);
        history.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCaloriesBurnedHistory_ShouldReturn200_WhenCalled()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/trainings/calories");

        // Assert
        var history = await response.ShouldHaveStatusCodeAndContent<
            List<WorkoutAggregatedValueDto>
        >(HttpStatusCode.OK);
        history.Should().NotBeNull();
    }
}
