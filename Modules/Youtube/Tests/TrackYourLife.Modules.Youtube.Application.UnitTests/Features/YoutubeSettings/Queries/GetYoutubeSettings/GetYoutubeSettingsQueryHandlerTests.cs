using MediatR;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.EnsureDefaultYoutubeCategories;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeSettings.Queries.GetYoutubeSettings;

public sealed class GetYoutubeSettingsQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeSettingsQuery _youtubeSettingsQuery;
    private readonly IYoutubeCategoriesQuery _youtubeCategoriesQuery;
    private readonly IYoutubeChannelsQuery _youtubeChannelsQuery;
    private readonly ISender _sender;
    private readonly GetYoutubeSettingsQueryHandler _handler;

    public GetYoutubeSettingsQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeSettingsQuery = Substitute.For<IYoutubeSettingsQuery>();
        _youtubeCategoriesQuery = Substitute.For<IYoutubeCategoriesQuery>();
        _youtubeChannelsQuery = Substitute.For<IYoutubeChannelsQuery>();
        _sender = Substitute.For<ISender>();

        _handler = new GetYoutubeSettingsQueryHandler(
            _userIdentifierProvider,
            _youtubeSettingsQuery,
            _youtubeCategoriesQuery,
            _youtubeChannelsQuery,
            _sender
        );
    }

    [Fact]
    public async Task Handle_WhenCategoriesExist_ReturnsPolicyReadModel()
    {
        var userId = UserId.NewId();
        var query = new GetYoutubeSettingsQuery();
        var utc = DateTime.UtcNow;
        var settings = new YoutubeSettingReadModel(
            YoutubeSettingsId.NewId(),
            userId,
            SettingsPasswordHash: "hash",
            CreatedOnUtc: utc,
            ModifiedOnUtc: null
        );

        var catId = YoutubeCategoryId.NewId();
        var categories = new List<YoutubeCategoryReadModel>
        {
            new(
                catId,
                userId,
                "A",
                MaxVideosPerDay: 5,
                DisplayOrder: 0,
                CreatedOnUtc: utc,
                ModifiedOnUtc: null
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeSettingsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);
        _youtubeCategoriesQuery
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);

        _youtubeChannelsQuery
            .CountByUserIdGroupedByCategoryAsync(userId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    (IReadOnlyDictionary<YoutubeCategoryId, int>)
                        new Dictionary<YoutubeCategoryId, int>()
                )
            );

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Settings.Should().Be(settings);
        result.Value.Categories.Should().BeEquivalentTo(categories);
        result.Value.SubscribedChannelCountsByCategoryId.Should().NotBeNull().And.BeEmpty();
        await _sender
            .DidNotReceive()
            .Send(Arg.Any<EnsureDefaultYoutubeCategoriesCommand>(), Arg.Any<CancellationToken>());
        await _youtubeChannelsQuery
            .Received(1)
            .CountByUserIdGroupedByCategoryAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoriesEmpty_SeedsThenReturnsCategories()
    {
        var userId = UserId.NewId();
        var utc = DateTime.UtcNow;
        var catId = YoutubeCategoryId.NewId();
        var afterSeed = new List<YoutubeCategoryReadModel>
        {
            new(
                catId,
                userId,
                "A",
                MaxVideosPerDay: 5,
                DisplayOrder: 0,
                CreatedOnUtc: utc,
                ModifiedOnUtc: null
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeSettingsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((YoutubeSettingReadModel?)null);

        _youtubeCategoriesQuery
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult<IReadOnlyList<YoutubeCategoryReadModel>>(
                    new List<YoutubeCategoryReadModel>()
                ),
                Task.FromResult<IReadOnlyList<YoutubeCategoryReadModel>>(afterSeed)
            );

        _sender
            .Send(Arg.Any<EnsureDefaultYoutubeCategoriesCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _youtubeChannelsQuery
            .CountByUserIdGroupedByCategoryAsync(userId, Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(
                    (IReadOnlyDictionary<YoutubeCategoryId, int>)
                        new Dictionary<YoutubeCategoryId, int>()
                )
            );

        var result = await _handler.Handle(new GetYoutubeSettingsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Settings.Should().BeNull();
        result.Value.Categories.Should().HaveCount(1);
        await _sender
            .Received(1)
            .Send(Arg.Any<EnsureDefaultYoutubeCategoriesCommand>(), Arg.Any<CancellationToken>());
        await _youtubeChannelsQuery
            .Received(1)
            .CountByUserIdGroupedByCategoryAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenChannelsExist_ReturnsSubscribedChannelCounts()
    {
        var userId = UserId.NewId();
        var utc = DateTime.UtcNow;
        var settings = new YoutubeSettingReadModel(
            YoutubeSettingsId.NewId(),
            userId,
            SettingsPasswordHash: null,
            CreatedOnUtc: utc,
            ModifiedOnUtc: null
        );

        var catId = YoutubeCategoryId.NewId();
        var categories = new List<YoutubeCategoryReadModel>
        {
            new(
                catId,
                userId,
                "A",
                MaxVideosPerDay: 5,
                DisplayOrder: 0,
                CreatedOnUtc: utc,
                ModifiedOnUtc: null
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeSettingsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);
        _youtubeCategoriesQuery
            .ListByUserIdOrderedAsync(userId, Arg.Any<CancellationToken>())
            .Returns(categories);

        var counts = new Dictionary<YoutubeCategoryId, int> { [catId] = 4 };
        _youtubeChannelsQuery
            .CountByUserIdGroupedByCategoryAsync(userId, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult((IReadOnlyDictionary<YoutubeCategoryId, int>)counts));

        var result = await _handler.Handle(new GetYoutubeSettingsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result
            .Value!.SubscribedChannelCountsByCategoryId.Should()
            .ContainKey(catId)
            .WhoseValue.Should()
            .Be(4);
    }
}
