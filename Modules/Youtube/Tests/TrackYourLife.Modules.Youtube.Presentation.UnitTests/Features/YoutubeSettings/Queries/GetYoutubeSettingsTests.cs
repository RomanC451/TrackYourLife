using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Models;
using TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.YoutubeSettings.Queries;

public class GetYoutubeSettingsTests
{
    private readonly ISender _sender;
    private readonly GetYoutubeSettings _endpoint;

    public GetYoutubeSettingsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetYoutubeSettings(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDto()
    {
        // Arrange
        YoutubeSettingReadModel readModel = new YoutubeSettingReadModel(
            YoutubeSettingsId.NewId(),
            UserId.NewId(),
            MaxEntertainmentVideosPerDay: 5,
            SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 7,
            LastSettingsChangeUtc: DateTime.UtcNow.AddDays(-1),
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null,
            CreatedOnUtc: DateTime.UtcNow,
            ModifiedOnUtc: null
        );

        _sender
            .Send(Arg.Any<GetYoutubeSettingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<YoutubeSettingReadModel?>(readModel));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<YoutubeSettingsDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.MaxEntertainmentVideosPerDay.Should().Be(5);
        okResult
            .Value.SettingsChangeFrequency.Should()
            .Be(SettingsChangeFrequency.OnceEveryFewDays);

        await _sender
            .Received(1)
            .Send(Arg.Is<GetYoutubeSettingsQuery>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryReturnsNull_ShouldReturnOkWithNull()
    {
        // Arrange
        _sender
            .Send(Arg.Any<GetYoutubeSettingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<YoutubeSettingReadModel?>(null));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<YoutubeSettingsDto>>().Subject;
        okResult.Value.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetYoutubeSettingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<YoutubeSettingReadModel?>(error));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
