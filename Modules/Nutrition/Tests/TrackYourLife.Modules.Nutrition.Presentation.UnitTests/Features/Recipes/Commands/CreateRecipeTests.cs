using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.Recipes.Commands;

public class CreateRecipeTests
{
    private readonly ISender _sender;
    private readonly CreateRecipe _endpoint;

    public CreateRecipeTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new CreateRecipe(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var recipeId = RecipeId.NewId();
        _sender
            .Send(Arg.Any<CreateRecipeCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(recipeId));

        var request = new CreateRecipeRequest(Name: "Test Recipe", Portions: 4, Weight: 500.0f);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(recipeId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<CreateRecipeCommand>(c =>
                    c.Name == "Test Recipe"
                    && c.Portions == 4
                    && Math.Abs(c.Weight - 500.0f) < 0.000001
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("ValidationError", "Invalid recipe data");
        _sender
            .Send(Arg.Any<CreateRecipeCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<RecipeId>(error));

        var request = new CreateRecipeRequest(Name: "Test Recipe", Portions: 4, Weight: 500.0f);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
