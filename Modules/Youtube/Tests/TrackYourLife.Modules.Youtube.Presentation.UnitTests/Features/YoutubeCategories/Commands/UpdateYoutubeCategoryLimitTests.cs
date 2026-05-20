using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryLimit;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeCategories.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeCategories.Commands;

public class UpdateYoutubeCategoryLimitTests
{
    private readonly ISender _sender;
    private readonly UpdateYoutubeCategoryLimit _endpoint;

    public UpdateYoutubeCategoryLimitTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new UpdateYoutubeCategoryLimit(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        var categoryId = YoutubeCategoryId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", categoryId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        _sender
            .Send(Arg.Any<UpdateYoutubeCategoryLimitCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var request = new UpdateYoutubeCategoryLimitRequest(7);

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateYoutubeCategoryLimitCommand>(c =>
                    c.CategoryId == categoryId && c.MaxVideosPerDay == 7
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        var categoryId = YoutubeCategoryId.NewId();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.RouteValues = new RouteValueDictionary
        {
            { "id", categoryId.Value.ToString() },
        };
        _endpoint.SetHttpContext(httpContext);

        var error = new Error("NotFound", "Category not found");
        _sender
            .Send(Arg.Any<UpdateYoutubeCategoryLimitCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        var request = new UpdateYoutubeCategoryLimitRequest(5);

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
