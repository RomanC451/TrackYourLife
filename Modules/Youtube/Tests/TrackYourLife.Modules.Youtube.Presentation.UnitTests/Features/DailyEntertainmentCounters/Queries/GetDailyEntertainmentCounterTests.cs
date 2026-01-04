using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Youtube.Application.Features.DailyEntertainmentCounters.Queries.GetDailyEntertainmentCounter;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyEntertainmentCounters.Models;
using TrackYourLife.Modules.Youtube.Presentation.Features.DailyEntertainmentCounters.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Presentation.UnitTests.Features.DailyEntertainmentCounters.Queries;

public class GetDailyEntertainmentCounterTests
{
    private readonly ISender _sender;
    private readonly GetDailyEntertainmentCounter _endpoint;

    public GetDailyEntertainmentCounterTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetDailyEntertainmentCounter(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithDto()
    {
        // Arrange
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var readModel = new DailyEntertainmentCounterReadModel(
            DailyEntertainmentCounterId.NewId(),
            UserId.NewId(),
            date,
            VideosWatchedCount: 5
        );

        _sender
            .Send(Arg.Any<GetDailyEntertainmentCounterQuery>(), Arg.Any<CancellationToken>())
            .Returns(
                Task.FromResult(Result.Success<DailyEntertainmentCounterReadModel?>(readModel))
            );

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<DailyEntertainmentCounterDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Date.Should().Be(date);
        okResult.Value.VideosWatchedCount.Should().Be(5);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetDailyEntertainmentCounterQuery>(q => q != null),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryReturnsNull_ShouldReturnOkWithNull()
    {
        // Arrange
        _sender
            .Send(Arg.Any<GetDailyEntertainmentCounterQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<DailyEntertainmentCounterReadModel?>(null)));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<DailyEntertainmentCounterDto>>().Subject;
        okResult.Value.Should().BeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("TestError", "Test error message");
        _sender
            .Send(Arg.Any<GetDailyEntertainmentCounterQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<DailyEntertainmentCounterReadModel?>(error)));

        var request = new EmptyRequest();

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
