using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeSettings.Queries.GetYoutubeSettings;

public sealed class GetYoutubeSettingsQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeSettingsQuery _youtubeSettingsQuery;
    private readonly GetYoutubeSettingsQueryHandler _handler;

    public GetYoutubeSettingsQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeSettingsQuery = Substitute.For<IYoutubeSettingsQuery>();

        _handler = new GetYoutubeSettingsQueryHandler(
            _userIdentifierProvider,
            _youtubeSettingsQuery
        );
    }

    [Fact]
    public async Task Handle_WhenSettingsExist_ReturnsSettings()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetYoutubeSettingsQuery();
        var settings = new YoutubeSettingReadModel(
            YoutubeSettingsId.NewId(),
            userId,
            MaxEntertainmentVideosPerDay: 5,
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: null,
            LastSettingsChangeUtc: null,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null,
            CreatedOnUtc: DateTime.UtcNow,
            ModifiedOnUtc: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeSettingsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(settings);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(settings);
        await _youtubeSettingsQuery
            .Received(1)
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSettingsDoNotExist_ReturnsNull()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetYoutubeSettingsQuery();

        _userIdentifierProvider.UserId.Returns(userId);
        _youtubeSettingsQuery
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((YoutubeSettingReadModel?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}
