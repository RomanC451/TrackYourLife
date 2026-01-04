using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;

namespace TrackYourLife.Modules.Nutrition.FunctionalTests.Features;

[Collection("Nutrition Integration Tests")]
public class DailyNutritionOverviewsTests(NutritionFunctionalTestWebAppFactory factory)
    : NutritionBaseIntegrationTest(factory)
{
    [Fact]
    public async Task GetDailyNutritionOverviewsByDateRange_WithValidData_ShouldReturnOverviews()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        // Create overviews for each day in the range
        for (int i = 0; i <= 2; i++)
        {
            var overview = DailyNutritionOverviewFaker.Generate(
                userId: _user.Id,
                date: startDate.AddDays(i)
            );
            await _nutritionWriteDbContext.DailyNutritionOverviews.AddAsync(overview);
        }
        await _nutritionWriteDbContext.SaveChangesAsync();

        var url =
            $"/api/daily-nutrition-overviews/range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var overviews = await response.Content.ReadFromJsonAsync<List<DailyNutritionOverviewDto>>();
        overviews.Should().NotBeNull();
        overviews!.Count.Should().Be(3);
        overviews
            .Should()
            .AllSatisfy(o =>
            {
                o.StartDate.Should().BeOnOrAfter(startDate);
                o.EndDate.Should().BeOnOrBefore(endDate);
                o.CaloriesGoal.Should().BeGreaterThan(0);
                o.CarbohydratesGoal.Should().BeGreaterThan(0);
                o.FatGoal.Should().BeGreaterThan(0);
                o.ProteinGoal.Should().BeGreaterThan(0);
                o.NutritionalContent.Should().NotBeNull();
            });
    }

    [Fact]
    public async Task GetDailyNutritionOverviewsByDateRange_WithNoData_ShouldReturnEmptyList()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(2);

        var url =
            $"/api/daily-nutrition-overviews/range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.OK);
        var overviews = await response.Content.ReadFromJsonAsync<List<DailyNutritionOverviewDto>>();
        overviews.Should().NotBeNull();
        overviews!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDailyNutritionOverviewsByDateRange_WithInvalidDateRange_ShouldReturnBadRequest()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(-1); // Invalid: end date before start date
        var url =
            $"/api/daily-nutrition-overviews/range?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Validation Error");
    }

    [Fact]
    public async Task GetDailyNutritionOverviewsByDateRange_WithMissingDates_ShouldReturnBadRequest()
    {
        var url = "/api/daily-nutrition-overviews/range";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Title.Should().Be("Validation Error");
    }
}
