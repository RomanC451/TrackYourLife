using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionDiariesByDate;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.NutritionDiaries.Queries;

public class GetNutritionDiariesByDateTests
{
    private readonly ISender _sender;
    private readonly GetNutritionDiariesByDate _endpoint;

    public GetNutritionDiariesByDateTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetNutritionDiariesByDate(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithResponse()
    {
        // Arrange
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "date", date.ToString("yyyy-MM-dd") },
        };
        _endpoint.SetHttpContext(httpContext);

        var foodReadModel = new FoodReadModel(foodId, "Test Food", "Generic", "Test Brand", "US")
        {
            NutritionalContents = new(),
            FoodServingSizes = [],
        };

        var servingSizeReadModel = new ServingSizeReadModel(
            servingSizeId,
            1.0f,
            "cup",
            100.0f,
            null
        );

        var foodDiaryReadModel = new FoodDiaryReadModel(
            NutritionDiaryId.NewId(),
            userId,
            2.0f,
            MealTypes.Breakfast,
            date,
            DateTime.UtcNow
        )
        {
            Food = foodReadModel,
            ServingSize = servingSizeReadModel,
        };

        var foodDiaries = new Dictionary<MealTypes, List<FoodDiaryReadModel>>
        {
            { MealTypes.Breakfast, [foodDiaryReadModel] },
        };

        var recipeDiaries = new Dictionary<MealTypes, List<RecipeDiaryReadModel>>();

        var resultTuple = (foodDiaries, recipeDiaries);

        _sender
            .Send(Arg.Any<GetNutritionDiariesByDateQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(resultTuple));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<GetNutritionDiariesByDateResponse>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Diaries.Should().ContainKey(MealTypes.Breakfast);
        okResult.Value.Diaries[MealTypes.Breakfast].Should().HaveCount(1);

        await _sender
            .Received(1)
            .Send(Arg.Any<GetNutritionDiariesByDateQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "date", date.ToString("yyyy-MM-dd") },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("Error", "Failed to get nutrition diaries");
        _sender
            .Send(Arg.Any<GetNutritionDiariesByDateQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Failure<(
                    Dictionary<MealTypes, List<FoodDiaryReadModel>>,
                    Dictionary<MealTypes, List<RecipeDiaryReadModel>>
                )>(error)
            );

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
