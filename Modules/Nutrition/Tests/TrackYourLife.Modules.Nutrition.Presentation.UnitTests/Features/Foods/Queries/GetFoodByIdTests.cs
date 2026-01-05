using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Foods.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.Foods.Queries;

public class GetFoodByIdTests
{
    private readonly ISender _sender;
    private readonly GetFoodById _endpoint;

    public GetFoodByIdTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetFoodById(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDto()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", foodId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var foodReadModel = new FoodReadModel(foodId, "Test Food", "Generic", "Test Brand", "US")
        {
            NutritionalContents = new(),
            FoodServingSizes = [],
        };

        _sender
            .Send(Arg.Any<GetFoodByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(foodReadModel));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<FoodDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(foodId);
        okResult.Value.Name.Should().Be("Test Food");

        await _sender.Received(1).Send(Arg.Any<GetFoodByIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var foodId = FoodId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", foodId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Food not found");
        _sender
            .Send(Arg.Any<GetFoodByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<FoodReadModel>(error));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
