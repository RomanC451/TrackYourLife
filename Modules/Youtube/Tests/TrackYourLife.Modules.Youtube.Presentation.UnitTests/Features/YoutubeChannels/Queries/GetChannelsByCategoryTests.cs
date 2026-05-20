using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Models;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeChannels.Queries;

public class GetChannelsByCategoryTests
{
    private readonly ISender _sender;
    private readonly GetChannelsByCategory _endpoint;

    public GetChannelsByCategoryTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetChannelsByCategory(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDtos()
    {
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "UCtest123",
                "Test Channel 1",
                "https://example.com/thumb1.jpg",
                catId,
                "Cat",
                DateTime.UtcNow,
                null
            ),
        };

        _sender
            .Send(Arg.Any<GetChannelsByCategoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IEnumerable<YoutubeChannelReadModel>>(channels)));

        var request = new GetChannelsByCategoryRequest { YoutubeCategoryId = catId.Value };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<IEnumerable<YoutubeChannelDto>>>().Subject;
        var dtos = okResult.Value!.ToList();
        dtos.Should().HaveCount(1);
        dtos[0].Name.Should().Be("Test Channel 1");
        dtos[0].YoutubeCategoryId.Should().Be(catId.Value);
        dtos[0].CategoryName.Should().Be("Cat");

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetChannelsByCategoryQuery>(q => q.YoutubeCategoryId == catId),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryIsNull_ShouldPassNullToQuery()
    {
        _sender
            .Send(Arg.Any<GetChannelsByCategoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    Result.Success<IEnumerable<YoutubeChannelReadModel>>(new List<YoutubeChannelReadModel>())
                )
            );

        var request = new GetChannelsByCategoryRequest { YoutubeCategoryId = null };

        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        await _sender
            .Received(1)
            .Send(Arg.Is<GetChannelsByCategoryQuery>(q => q.YoutubeCategoryId == null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetChannelsByCategoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<YoutubeChannelReadModel>>(error)));

        var request = new GetChannelsByCategoryRequest { YoutubeCategoryId = Guid.NewGuid() };

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
