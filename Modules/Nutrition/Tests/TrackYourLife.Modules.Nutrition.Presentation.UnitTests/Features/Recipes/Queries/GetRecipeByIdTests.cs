using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipeById;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.Recipes.Queries;

public class GetRecipeByIdTests
{
    private readonly ISender _sender;
    private readonly GetRecipeById _endpoint;

    public GetRecipeByIdTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetRecipeById(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDto()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var userId = UserId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", recipeId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var recipeReadModel = new RecipeReadModel(recipeId, userId, "Test Recipe", 4, 500.0f, false)
        {
            Ingredients = [],
            NutritionalContents = new(),
            ServingSizes = [],
        };

        _sender
            .Send(Arg.Any<GetRecipeByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(recipeReadModel));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<RecipeDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(recipeId);
        okResult.Value.Name.Should().Be("Test Recipe");

        await _sender
            .Received(1)
            .Send(Arg.Is<GetRecipeByIdQuery>(q => q.Id == recipeId), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", recipeId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Recipe not found");
        _sender
            .Send(Arg.Any<GetRecipeByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<RecipeReadModel>(error));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
