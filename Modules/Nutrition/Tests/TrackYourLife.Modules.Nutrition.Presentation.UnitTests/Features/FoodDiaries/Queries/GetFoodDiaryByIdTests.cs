using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Queries.GetFoodDiaryById;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.FoodDiaries.Queries;

public class GetFoodDiaryByIdTests
{
    private readonly ISender _sender;
    private readonly GetFoodDiaryById _endpoint;

    public GetFoodDiaryByIdTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetFoodDiaryById(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDto()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", nutritionDiaryId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var foodReadModel = new FoodReadModel(foodId, "Test Food", "Generic", "Test Brand", "US")
        {
            NutritionalContents = new(),
        };

        var servingSizeReadModel = new ServingSizeReadModel(
            servingSizeId,
            1.0f,
            "cup",
            100.0f,
            null
        );

        var foodDiaryReadModel = new FoodDiaryReadModel(
            nutritionDiaryId,
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

        _sender
            .Send(Arg.Any<GetFoodDiaryByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(foodDiaryReadModel));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<FoodDiaryDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(nutritionDiaryId);
        okResult.Value.Quantity.Should().Be(2.0f);
        okResult.Value.MealType.Should().Be(MealTypes.Breakfast);

        await _sender
            .Received(1)
            .Send(Arg.Any<GetFoodDiaryByIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", nutritionDiaryId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Food diary not found");
        _sender
            .Send(Arg.Any<GetFoodDiaryByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<FoodDiaryReadModel>(error));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
