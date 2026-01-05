using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.AddIngredient;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.Recipes.Commands;

public class AddIngredientTests
{
    private readonly ISender _sender;
    private readonly AddIngredient _endpoint;

    public AddIngredientTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new AddIngredient(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var ingredientId = IngredientId.NewId();
        var foodId = FoodId.NewId();
        var servingSizeId = ServingSizeId.NewId();

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "recipeId", recipeId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<AddIngredientCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(ingredientId));

        var request = new AddIngredientRequest(
            FoodId: foodId,
            ServingSizeId: servingSizeId,
            Quantity: 2.0f
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(ingredientId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<AddIngredientCommand>(c =>
                    c.RecipeId == recipeId
                    && c.FoodId == foodId
                    && c.ServingSizeId == servingSizeId
                    && Math.Abs(c.Quantity - 2.0f) < 0.000001
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "recipeId", recipeId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Recipe not found");
        _sender
            .Send(Arg.Any<AddIngredientCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<IngredientId>(error));

        var request = new AddIngredientRequest(
            FoodId: FoodId.NewId(),
            ServingSizeId: ServingSizeId.NewId(),
            Quantity: 1.0f
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
