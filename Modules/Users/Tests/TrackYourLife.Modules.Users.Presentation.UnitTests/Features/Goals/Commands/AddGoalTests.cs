using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Goals.Commands;

public class AddGoalTests
{
    private readonly ISender _sender;
    private readonly AddGoal _endpoint;

    public AddGoalTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new AddGoal(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreatedWithIdResponse()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var request = new AddGoalRequest(
            Value: 2000,
            Type: GoalType.Calories,
            Period: GoalPeriod.Day,
            StartDate: new DateOnly(2024, 1, 1),
            Force: false,
            EndDate: new DateOnly(2024, 1, 31)
        );

        _sender
            .Send(Arg.Any<AddGoalCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(goalId)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var createdResult = result.Should().BeOfType<Created<IdResponse>>().Subject;
        createdResult.Value.Should().NotBeNull();
        createdResult.Value!.Id.Should().Be(goalId);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<AddGoalCommand>(c =>
                    c.Value == request.Value
                    && c.Type == request.Type
                    && c.PerPeriod == request.Period
                    && c.StartDate == request.StartDate
                    && c.EndDate == request.EndDate
                    && c.Force == request.Force
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenForceIsNull_ShouldDefaultToFalse()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var request = new AddGoalRequest(
            Value: 2000,
            Type: GoalType.Calories,
            Period: GoalPeriod.Day,
            StartDate: new DateOnly(2024, 1, 1),
            Force: null,
            EndDate: null
        );

        _sender
            .Send(Arg.Any<AddGoalCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(goalId)));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(Arg.Is<AddGoalCommand>(c => !c.Force), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new AddGoalRequest(
            Value: 2000,
            Type: GoalType.Calories,
            Period: GoalPeriod.Day,
            StartDate: new DateOnly(2024, 1, 1),
            Force: false,
            EndDate: new DateOnly(2024, 1, 31)
        );

        var error = new Error("Validation", "Invalid goal parameters");
        _sender
            .Send(Arg.Any<AddGoalCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<GoalId>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
