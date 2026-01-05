using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Goals.Commands;

public class UpdateGoalTests
{
    private readonly ISender _sender;
    private readonly UpdateGoal _endpoint;

    public UpdateGoalTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new UpdateGoal(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var request = new UpdateGoalRequest(
            Id: goalId,
            Type: GoalType.Calories,
            Value: 2500,
            Period: GoalPeriod.Day,
            StartDate: new DateOnly(2024, 1, 1),
            Force: false,
            EndDate: new DateOnly(2024, 1, 31)
        );

        _sender
            .Send(Arg.Any<UpdateGoalCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateGoalCommand>(c =>
                    c.Id == request.Id
                    && c.Type == request.Type
                    && c.Value == request.Value
                    && c.PerPeriod == request.Period
                    && c.StartDate == request.StartDate
                    && c.EndDate == request.EndDate
                    && c.Force == request.Force),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenForceIsNull_ShouldDefaultToFalse()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var request = new UpdateGoalRequest(
            Id: goalId,
            Type: GoalType.Calories,
            Value: 2500,
            Period: GoalPeriod.Day,
            StartDate: new DateOnly(2024, 1, 1),
            Force: null,
            EndDate: null
        );

        _sender
            .Send(Arg.Any<UpdateGoalCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateGoalCommand>(c => !c.Force),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var goalId = GoalId.NewId();
        var request = new UpdateGoalRequest(
            Id: goalId,
            Type: GoalType.Calories,
            Value: 2500,
            Period: GoalPeriod.Day,
            StartDate: new DateOnly(2024, 1, 1),
            Force: false,
            EndDate: new DateOnly(2024, 1, 31)
        );

        var error = new Error("Validation", "Invalid goal parameters");
        _sender
            .Send(Arg.Any<UpdateGoalCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
