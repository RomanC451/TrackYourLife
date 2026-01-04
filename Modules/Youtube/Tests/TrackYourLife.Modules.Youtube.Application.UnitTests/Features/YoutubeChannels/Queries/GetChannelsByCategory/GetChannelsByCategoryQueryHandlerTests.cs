using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeChannels.Queries.GetChannelsByCategory;

public sealed class GetChannelsByCategoryQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly GetChannelsByCategoryQueryHandler _handler;

    public GetChannelsByCategoryQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();

        _handler = new GetChannelsByCategoryQueryHandler(
            _userIdentifierProvider,
            _youtubeChannelsQuery
        );
    }

    [Fact]
    public async Task Handle_WhenCategorySpecified_ReturnsChannelsByCategory()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetChannelsByCategoryQuery(Category: VideoCategory.Educational);
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-1",
                "Channel 1",
                "thumbnail",
                VideoCategory.Educational,
                DateTime.UtcNow,
                null
            )
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAndCategoryAsync(userId, VideoCategory.Educational, Arg.Any<CancellationToken>())
            .Returns(channels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        await _youtubeChannelsQuery
            .Received(1)
            .GetByUserIdAndCategoryAsync(userId, VideoCategory.Educational, Arg.Any<CancellationToken>());
        await _youtubeChannelsQuery
            .DidNotReceive()
            .GetByUserIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryNotSpecified_ReturnsAllChannels()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetChannelsByCategoryQuery(Category: null);
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-1",
                "Channel 1",
                "thumbnail",
                VideoCategory.Educational,
                DateTime.UtcNow,
                null
            ),
            new(
                YoutubeChannelId.NewId(),
                userId,
                "channel-2",
                "Channel 2",
                "thumbnail",
                VideoCategory.Entertainment,
                DateTime.UtcNow,
                null
            )
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(channels);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        await _youtubeChannelsQuery
            .Received(1)
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>());
        await _youtubeChannelsQuery
            .DidNotReceive()
            .GetByUserIdAndCategoryAsync(Arg.Any<UserId>(), Arg.Any<VideoCategory>(), Arg.Any<CancellationToken>());
    }
}
