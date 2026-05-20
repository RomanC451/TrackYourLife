using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.CreateYoutubeCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeCategories.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeCategories.Commands;

public class CreateYoutubeCategoryTests
{
    private readonly ISender _sender;
    private readonly CreateYoutubeCategory _endpoint;

    public CreateYoutubeCategoryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new CreateYoutubeCategory(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        var categoryId = YoutubeCategoryId.NewId();
        _sender
            .Send(Arg.Any<CreateYoutubeCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(categoryId));

        var request = new CreateYoutubeCategoryRequest("Custom", 4);

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value!.Id.Should().Be(categoryId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<CreateYoutubeCategoryCommand>(c => c.Name == "Custom" && c.MaxVideosPerDay == 4),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        var error = new Error("ConflictError", "Duplicate name");
        _sender
            .Send(Arg.Any<CreateYoutubeCategoryCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<YoutubeCategoryId>(error));

        var request = new CreateYoutubeCategoryRequest("Custom", 4);

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
