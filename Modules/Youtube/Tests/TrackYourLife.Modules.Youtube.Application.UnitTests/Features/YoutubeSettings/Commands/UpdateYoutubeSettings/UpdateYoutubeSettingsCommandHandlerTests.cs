using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;

public sealed class UpdateYoutubeSettingsCommandHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IYoutubeSettingsRepository _youtubeSettingsRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly UpdateYoutubeSettingsCommandHandler _handler;

    public UpdateYoutubeSettingsCommandHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _youtubeSettingsRepository = Substitute.For<IYoutubeSettingsRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new UpdateYoutubeSettingsCommandHandler(
            _userIdentifierProvider,
            _youtubeSettingsRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenSettingsDoNotExist_CreatesNewSettings()
    {
        // Arrange
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var command = new UpdateYoutubeSettingsCommand(
            MaxEntertainmentVideosPerDay: 5,
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 1,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns((Domain.Features.YoutubeSettings.YoutubeSetting?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _youtubeSettingsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<Domain.Features.YoutubeSettings.YoutubeSetting>(s =>
                    s.MaxEntertainmentVideosPerDay == 5
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task Handle_WhenSettingsCannotBeChanged_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var command = new UpdateYoutubeSettingsCommand(
            MaxEntertainmentVideosPerDay: 5,
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 1,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        var existingSettings = Domain.Features.YoutubeSettings.YoutubeSetting.Create(
            YoutubeSettingsId.NewId(),
            userId,
            maxEntertainmentVideosPerDay: 5,
            settingsChangeFrequency: SettingsChangeFrequency.SpecificDayOfWeek,
            daysBetweenChanges: null,
            lastSettingsChangeUtc: utcNow.AddDays(-1),
            specificDayOfWeek: DayOfWeek.Monday,
            specificDayOfMonth: null,
            createdOnUtc: utcNow.AddDays(-1)
        ).Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(existingSettings);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        _youtubeSettingsRepository.DidNotReceive().Update(Arg.Any<Domain.Features.YoutubeSettings.YoutubeSetting>());
    }

    [Fact]
    public async Task Handle_WhenSettingsCanBeChanged_UpdatesSettings()
    {
        // Arrange
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var command = new UpdateYoutubeSettingsCommand(
            MaxEntertainmentVideosPerDay: 10,
            SettingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 1,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        var existingSettings = Domain.Features.YoutubeSettings.YoutubeSetting.Create(
            YoutubeSettingsId.NewId(),
            userId,
            maxEntertainmentVideosPerDay: 5,
            settingsChangeFrequency: SettingsChangeFrequency.OnceEveryFewDays,
            daysBetweenChanges: 1,
            lastSettingsChangeUtc: utcNow.AddDays(-2),
            specificDayOfWeek: null,
            specificDayOfMonth: null,
            createdOnUtc: utcNow.AddDays(-2)
        ).Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _youtubeSettingsRepository
            .GetByUserIdAsync(userId, Arg.Any<CancellationToken>())
            .Returns(existingSettings);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _youtubeSettingsRepository.Received(1).Update(existingSettings);
    }
}
