using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Queries.GetChannelsByCategory;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
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
        _handler = new GetChannelsByCategoryQueryHandler(_userIdentifierProvider, _youtubeChannelsQuery);
    }

    [Fact]
    public async Task Handle_WhenCategorySpecified_UsesFilteredQuery()
    {
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "UC1",
                "A",
                null,
                catId,
                "Cat",
                false,
                DateTime.UtcNow,
                null
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery
            .GetByUserIdAndYoutubeCategoryIdAsync(userId, catId, Arg.Any<CancellationToken>())
            .Returns(channels);

        var result = await _handler.Handle(new GetChannelsByCategoryQuery(catId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(channels);
        await _youtubeChannelsQuery
            .DidNotReceive()
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryNull_ReturnsAllUserChannels()
    {
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var channels = new List<YoutubeChannelReadModel>
        {
            new(
                YoutubeChannelId.NewId(),
                userId,
                "UC1",
                "A",
                null,
                catId,
                "Cat",
                false,
                DateTime.UtcNow,
                null
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeChannelsQuery.GetByUserIdAsync(userId, Arg.Any<CancellationToken>()).Returns(channels);

        var result = await _handler.Handle(new GetChannelsByCategoryQuery(null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(channels);
    }
}
