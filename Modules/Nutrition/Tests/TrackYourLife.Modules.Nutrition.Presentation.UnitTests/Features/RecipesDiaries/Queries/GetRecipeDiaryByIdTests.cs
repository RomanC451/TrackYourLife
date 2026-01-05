using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Queries.GetRecipeDiaryById;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.RecipesDiaries.Queries;

public class GetRecipeDiaryByIdTests
{
    private readonly ISender _sender;
    private readonly GetRecipeDiaryById _endpoint;

    public GetRecipeDiaryByIdTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetRecipeDiaryById(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDto()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var userId = UserId.NewId();
        var recipeId = RecipeId.NewId();
        var servingSizeId = ServingSizeId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", nutritionDiaryId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var recipeReadModel = new RecipeReadModel(recipeId, userId, "Test Recipe", 4, 500.0f, false)
        {
            Ingredients = [],
            NutritionalContents = new(),
            ServingSizes = [new ServingSizeReadModel(servingSizeId, 1.0f, "portion", 125.0f, null)],
        };

        var recipeDiaryReadModel = new RecipeDiaryReadModel(
            nutritionDiaryId,
            userId,
            servingSizeId,
            2.0f,
            MealTypes.Breakfast,
            date,
            DateTime.UtcNow
        )
        {
            Recipe = recipeReadModel,
        };

        _sender
            .Send(Arg.Any<GetRecipeDiaryByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(recipeDiaryReadModel));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<RecipeDiaryDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(nutritionDiaryId);
        okResult.Value.Quantity.Should().Be(2.0f);
        okResult.Value.MealType.Should().Be(MealTypes.Breakfast);

        await _sender
            .Received(1)
            .Send(Arg.Any<GetRecipeDiaryByIdQuery>(), Arg.Any<CancellationToken>());
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

        var error = new Error("NotFound", "Recipe diary not found");
        _sender
            .Send(Arg.Any<GetRecipeDiaryByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<RecipeDiaryReadModel>(error));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
