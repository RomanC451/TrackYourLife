using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;
using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Goals.Commands;

public class CalculateNutritionGoalsTests
{
    private readonly ISender _sender;
    private readonly CalculateNutritionGoals _endpoint;

    public CalculateNutritionGoalsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new CalculateNutritionGoals(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var request = new CalculateNutritionGoalsRequest(
            Age: 30,
            Weight: 75.5f,
            Height: 180.0f,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        _sender
            .Send(Arg.Any<CalculateNutritionGoalsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<CalculateNutritionGoalsCommand>(c =>
                    c.Age == request.Age
                    && Math.Abs(c.Weight - request.Weight) < 0.01f
                    && Math.Abs(c.Height - request.Height) < 0.01f
                    && c.Gender == request.Gender
                    && c.ActivityLevel == request.ActivityLevel
                    && c.FitnessGoal == request.FitnessGoal
                    && c.Force == request.Force),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CalculateNutritionGoalsRequest(
            Age: 30,
            Weight: 75.5f,
            Height: 180.0f,
            Gender: Gender.Male,
            ActivityLevel: ActivityLevel.ModeratelyActive,
            FitnessGoal: FitnessGoal.Maintain,
            Force: false
        );

        var error = new Error("Calculation", "Failed to calculate nutrition goals");
        _sender
            .Send(Arg.Any<CalculateNutritionGoalsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
