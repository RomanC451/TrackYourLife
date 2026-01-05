using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipesByUserId;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.Recipes.Queries;

public class GetRecipesTests
{
    private readonly ISender _sender;
    private readonly GetRecipesByUserId _endpoint;

    public GetRecipesTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetRecipesByUserId(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDtos()
    {
        // Arrange
        var userId = UserId.NewId();
        var recipes = new List<RecipeReadModel>
        {
            new(RecipeId.NewId(), userId, "Test Recipe 1", 4, 500.0f, false)
            {
                Ingredients = [],
                NutritionalContents = new(),
                ServingSizes = [],
            },
            new(RecipeId.NewId(), userId, "Test Recipe 2", 2, 300.0f, false)
            {
                Ingredients = [],
                NutritionalContents = new(),
                ServingSizes = [],
            },
        };

        _sender
            .Send(Arg.Any<GetRecipesByUserIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(recipes.AsReadOnly())));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<List<RecipeDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Should().HaveCount(2);

        await _sender
            .Received(1)
            .Send(Arg.Any<GetRecipesByUserIdQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("Error", "Failed to get recipes");
        _sender
            .Send(Arg.Any<GetRecipesByUserIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<ReadOnlyCollection<RecipeReadModel>>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
