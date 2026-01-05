using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.NutritionDiaries.Queries;

public class GetNutritionOverviewByPeriodTests
{
    private readonly ISender _sender;
    private readonly GetNutritionOverviewByPeriod _endpoint;

    public GetNutritionOverviewByPeriodTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetNutritionOverviewByPeriod(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithNutritionalContent()
    {
        // Arrange
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(7);
        var nutritionalContent = new NutritionalContent
        {
            Energy = new Energy { Value = 2000.0f, Unit = "Kcal" },
            Carbohydrates = 250.0f,
            Fat = 65.0f,
            Protein = 150.0f,
        };

        _sender
            .Send(Arg.Any<GetNutritionOverviewByPeriodQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(nutritionalContent));

        var request = new GetNutritionOverviewByPeriodRequest(
            StartDate: startDate,
            EndDate: endDate
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<NutritionalContent>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Energy.Value.Should().Be(2000.0f);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetNutritionOverviewByPeriodQuery>(q =>
                    q.StartDate == startDate && q.EndDate == endDate
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("Error", "Failed to get nutrition overview");
        _sender
            .Send(Arg.Any<GetNutritionOverviewByPeriodQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<NutritionalContent>(error));

        var request = new GetNutritionOverviewByPeriodRequest(
            StartDate: DateOnly.FromDateTime(DateTime.UtcNow),
            EndDate: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(7)
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
