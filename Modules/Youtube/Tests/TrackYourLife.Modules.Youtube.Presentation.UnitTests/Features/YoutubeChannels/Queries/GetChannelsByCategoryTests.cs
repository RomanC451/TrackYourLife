using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;
using TrackYourLife.Modules.Youtube.Domain.Core;
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
        // Arrange
        var userId = UserId.NewId();
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "UCtest123",
                "Test Channel 1",
                "https://example.com/thumb1.jpg",
                VideoCategory.Entertainment,
                DateTime.UtcNow,
                null
            ),
            new(
                YoutubeChannelId.NewId(),
                userId,
                "UCtest456",
                "Test Channel 2",
                null,
                VideoCategory.Entertainment,
                DateTime.UtcNow,
                null
            ),
        };

        _sender
            .Send(Arg.Any<GetChannelsByCategoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(Result.Success<IEnumerable<YoutubeChannelReadModel>>(channels))
            );

        var request = new GetChannelsByCategoryRequest { Category = VideoCategory.Entertainment };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<IEnumerable<YoutubeChannelDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        var dtos = okResult.Value!.ToList();
        dtos.Should().HaveCount(2);
        dtos[0].Name.Should().Be("Test Channel 1");
        dtos[0].Category.Should().Be(VideoCategory.Entertainment);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetChannelsByCategoryQuery>(q => q.Category == VideoCategory.Entertainment),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryIsNull_ShouldPassNullToQuery()
    {
        // Arrange
        _sender
            .Send(Arg.Any<GetChannelsByCategoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    Result.Success<IEnumerable<YoutubeChannelReadModel>>(
                        new List<YoutubeChannelReadModel>()
                    )
                )
            );

        var request = new GetChannelsByCategoryRequest { Category = null };

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetChannelsByCategoryQuery>(q => q.Category == null),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetChannelsByCategoryQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IEnumerable<YoutubeChannelReadModel>>(error)));

        var request = new GetChannelsByCategoryRequest { Category = VideoCategory.Educational };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
