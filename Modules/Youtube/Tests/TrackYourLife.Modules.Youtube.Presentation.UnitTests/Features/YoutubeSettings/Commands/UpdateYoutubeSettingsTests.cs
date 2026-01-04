using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Commands.UpdateYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Commands;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeSettings.Commands;

public class UpdateYoutubeSettingsTests
{
    private readonly ISender _sender;
    private readonly UpdateYoutubeSettings _endpoint;

    public UpdateYoutubeSettingsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new UpdateYoutubeSettings(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var settingsId = YoutubeSettingsId.NewId();
        _sender
            .Send(Arg.Any<UpdateYoutubeSettingsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<YoutubeSettingsId>(settingsId)));

        var request = new UpdateYoutubeSettingsRequest(
            MaxEntertainmentVideosPerDay: 5,
            SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 7,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(settingsId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateYoutubeSettingsCommand>(c =>
                    c.MaxEntertainmentVideosPerDay == 5
                    && c.SettingsChangeFrequency == SettingsChangeFrequency.OnceEveryFewDays
                    && c.DaysBetweenChanges == 7
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("ValidationError", "Invalid settings");
        _sender
            .Send(Arg.Any<UpdateYoutubeSettingsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<YoutubeSettingsId>(error)));

        var request = new UpdateYoutubeSettingsRequest(
            MaxEntertainmentVideosPerDay: 5,
            SettingsChangeFrequency.SpecificDayOfWeek,
            DaysBetweenChanges: null,
            SpecificDayOfWeek: DayOfWeek.Monday,
            SpecificDayOfMonth: null
        );

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldMapRequestToCommandCorrectly()
    {
        // Arrange
        var settingsId = YoutubeSettingsId.NewId();
        _sender
            .Send(Arg.Any<UpdateYoutubeSettingsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(settingsId)));

        var request = new UpdateYoutubeSettingsRequest(
            MaxEntertainmentVideosPerDay: 10,
            SettingsChangeFrequency.SpecificDayOfMonth,
            DaysBetweenChanges: null,
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: 15
        );

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateYoutubeSettingsCommand>(c =>
                    c.MaxEntertainmentVideosPerDay == 10
                    && c.SettingsChangeFrequency == SettingsChangeFrequency.SpecificDayOfMonth
                    && c.DaysBetweenChanges == null
                    && c.SpecificDayOfWeek == null
                    && c.SpecificDayOfMonth == 15
                ),
                Arg.Any<CancellationToken>()
            );
    }
}
