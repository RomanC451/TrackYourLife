using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetGoalByType;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Presentation.Features.Goals.Queries;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Goals.Queries;

public class GetGoalTests
{
    private readonly ISender _sender;
    private readonly GetGoal _endpoint;

    public GetGoalTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetGoal(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithGoalDto()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var userId = UserId.NewId();
        var goalType = GoalType.Calories;
        var date = new DateOnly(2024, 1, 15);

        var goalReadModel = new GoalReadModel(
            Id: goalId,
            UserId: userId,
            Value: 2000,
            Type: goalType,
            Period: GoalPeriod.Day,
            StartDate: new DateOnly(2024, 1, 1),
            EndDate: new DateOnly(2024, 1, 31)
        );

        var request = new GetGoalRequest
        {
            GoalType = goalType,
            Date = date
        };

        _sender
            .Send(Arg.Any<GetGoalByTypeQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(goalReadModel)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<GoalDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(goalId);
        okResult.Value.Type.Should().Be(goalType);
        okResult.Value.Value.Should().Be(2000);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetGoalByTypeQuery>(q =>
                    q.Type == goalType && q.Date == date),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new GetGoalRequest
        {
            GoalType = GoalType.Calories,
            Date = new DateOnly(2024, 1, 15)
        };

        var error = new Error("Goal", "Goal not found");
        _sender
            .Send(Arg.Any<GetGoalByTypeQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<GoalReadModel>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
