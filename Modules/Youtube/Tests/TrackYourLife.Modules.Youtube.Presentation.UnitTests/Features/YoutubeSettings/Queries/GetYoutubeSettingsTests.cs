using System.Collections.Generic;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeSettings.Queries.GetYoutubeSettings;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
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
        var userId = UserId.NewId();
        var utc = DateTime.UtcNow;
        var settings = new YoutubeSettingReadModel(
            YoutubeSettingsId.NewId(),
            userId,
            SettingsChangeFrequency.OnceEveryFewDays,
            DaysBetweenChanges: 7,
            LastSettingsChangeUtc: utc.AddDays(-1),
            SpecificDayOfWeek: null,
            SpecificDayOfMonth: null,
            CreatedOnUtc: utc,
            ModifiedOnUtc: null
        );
        var catId = YoutubeCategoryId.NewId();
        var categories = new List<YoutubeCategoryReadModel>
        {
            new(catId, userId, "A", MaxVideosPerDay: 5, DisplayOrder: 0, CreatedOnUtc: utc, ModifiedOnUtc: null),
        };
        var counts = new Dictionary<YoutubeCategoryId, int> { [catId] = 3 };
        var policy = new YoutubePolicyReadModel(settings, categories, counts);

        _sender
            .Send(Arg.Any<GetYoutubeSettingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(policy));

        var request = new EmptyRequest();

        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<YoutubeSettingsDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Categories.Should().HaveCount(1);
        okResult.Value.Categories[0].Id.Should().Be(catId.Value);
        okResult.Value.Categories[0].SubscribedChannelCount.Should().Be(3);
        okResult.Value.SettingsChangeFrequency.Should().Be(SettingsChangeFrequency.OnceEveryFewDays);

        await _sender
            .Received(1)
            .Send(Arg.Is<GetYoutubeSettingsQuery>(q => q != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenSettingsNull_ShouldReturnDtoWithNullFrequencyFields()
    {
        var userId = UserId.NewId();
        var utc = DateTime.UtcNow;
        var catId = YoutubeCategoryId.NewId();
        var categories = new List<YoutubeCategoryReadModel>
        {
            new(catId, userId, "A", MaxVideosPerDay: 5, DisplayOrder: 0, CreatedOnUtc: utc, ModifiedOnUtc: null),
        };
        var policy = new YoutubePolicyReadModel(null, categories);

        _sender
            .Send(Arg.Any<GetYoutubeSettingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(policy));

        var result = await _endpoint.ExecuteAsync(new EmptyRequest(), CancellationToken.None);

        var okResult = result.Should().BeOfType<Ok<YoutubeSettingsDto>>().Subject;
        okResult.Value!.SettingsChangeFrequency.Should().BeNull();
        okResult.Value.Categories.Should().HaveCount(1);
        okResult.Value.Categories[0].SubscribedChannelCount.Should().Be(0);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetYoutubeSettingsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<YoutubePolicyReadModel>(error));

        var result = await _endpoint.ExecuteAsync(new EmptyRequest(), CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
