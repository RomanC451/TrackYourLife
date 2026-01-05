using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Presentation.Features.DailyNutritionOverviews.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.DailyNutritionOverviews.Queries;

public class GetDailyNutritionOverviewsByDateRangeTests
{
    private readonly ISender _sender;
    private readonly GetDailyNutritionOverviewsByDateRange _endpoint;

    public GetDailyNutritionOverviewsByDateRangeTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetDailyNutritionOverviewsByDateRange(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDtos()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(7);
        var overviewType = OverviewType.Daily;
        var aggregationMode = AggregationMode.Sum;

        var overviews = new List<DailyNutritionOverviewDto>
        {
            new(
                DailyNutritionOverviewId.NewId(),
                startDate,
                endDate,
                new NutritionalContent
                {
                    Energy = new Energy { Value = 2000.0f, Unit = "Kcal" },
                    Carbohydrates = 250.0f,
                    Fat = 65.0f,
                    Protein = 150.0f,
                    Calcium = 100.0f,
                    Cholesterol = 100.0f,
                    Fiber = 100.0f,
                    Iron = 100.0f,
                    MonounsaturatedFat = 100.0f,
                    NetCarbs = 100.0f,
                    PolyunsaturatedFat = 100.0f,
                    Potassium = 100.0f,
                    SaturatedFat = 100.0f,
                    Sodium = 100.0f,
                    Sugar = 100.0f,
                    TransFat = 100.0f,
                    VitaminA = 100.0f,
                    VitaminC = 100.0f,
                },
                2000.0f,
                250.0f,
                65.0f,
                150.0f
            )
            {
                NutritionalContent = new(),
            },
        };

        _sender
            .Send(
                Arg.Any<GetDailyNutritionOverviewsByDateRangeQuery>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(Result.Success<IEnumerable<DailyNutritionOverviewDto>>(overviews))
            );

        var request = new GetDailyNutritionOverviewsByDateRangeRequest(
            StartDate: startDate,
            EndDate: endDate,
            OverviewType: overviewType,
            AggregationMode: aggregationMode
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<List<DailyNutritionOverviewDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Should().HaveCount(1);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetDailyNutritionOverviewsByDateRangeQuery>(q =>
                    q.StartDate == startDate
                    && q.EndDate == endDate
                    && q.OverviewType == overviewType
                    && q.AggregationMode == aggregationMode
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("Error", "Failed to get daily nutrition overviews");
        _sender
            .Send(
                Arg.Any<GetDailyNutritionOverviewsByDateRangeQuery>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(
                Task.FromResult(Result.Failure<IEnumerable<DailyNutritionOverviewDto>>(error))
            );

        var request = new GetDailyNutritionOverviewsByDateRangeRequest(
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7),
            OverviewType: OverviewType.Daily,
            AggregationMode: AggregationMode.Sum
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
