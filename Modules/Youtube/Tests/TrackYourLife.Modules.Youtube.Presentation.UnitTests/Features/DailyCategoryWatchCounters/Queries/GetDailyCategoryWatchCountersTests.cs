using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.DailyCategoryWatchCounters.Queries.GetDailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyCategoryWatchCounters.Models;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyCategoryWatchCounters.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.DailyCategoryWatchCounters.Queries;

public class GetDailyCategoryWatchCountersTests
{
    private readonly ISender _sender;
    private readonly GetDailyCategoryWatchCounters _endpoint;

    public GetDailyCategoryWatchCountersTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetDailyCategoryWatchCounters(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDtos()
    {
        var userId = UserId.NewId();
        var catId = YoutubeCategoryId.NewId();
        var rows = new List<DailyCategoryWatchCounterReadModel>
        {
            new(
                DailyCategoryWatchCounterId.NewId(),
                userId,
                new DateOnly(2026, 5, 14),
                catId,
                2
            ),
        };

        _sender
            .Send(Arg.Any<GetDailyCategoryWatchCountersQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<IReadOnlyList<DailyCategoryWatchCounterReadModel>>(rows)));

        var result = await _endpoint.ExecuteAsync(new EmptyRequest(), CancellationToken.None);

        var ok = result.Should().BeOfType<Ok<IEnumerable<DailyCategoryWatchCounterDto>>>().Subject;
        ok.Value!.Should().HaveCount(1);
        ok.Value!.First().VideosWatchedCount.Should().Be(2);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnBadRequest()
    {
        var error = new Error("E", "msg");
        _sender
            .Send(Arg.Any<GetDailyCategoryWatchCountersQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<IReadOnlyList<DailyCategoryWatchCounterReadModel>>(error)));

        var result = await _endpoint.ExecuteAsync(new EmptyRequest(), CancellationToken.None);

        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
