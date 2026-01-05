using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.DeleteRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Presentation.UnitTests;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.RecipesDiaries.Commands;

public class DeleteRecipeDiaryTests
{
    private readonly ISender _sender;
    private readonly DeleteRecipeDiary _endpoint;

    public DeleteRecipeDiaryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new DeleteRecipeDiary(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var nutritionDiaryId = NutritionDiaryId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", nutritionDiaryId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<DeleteRecipeDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<DeleteRecipeDiaryCommand>(c => c.Id == nutritionDiaryId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
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
            .Send(Arg.Any<DeleteRecipeDiaryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
