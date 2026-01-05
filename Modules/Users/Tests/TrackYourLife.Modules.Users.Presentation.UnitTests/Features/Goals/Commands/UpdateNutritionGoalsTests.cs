using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateNutritionGoals;
using TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Goals.Commands;

public class UpdateNutritionGoalsTests
{
    private readonly ISender _sender;
    private readonly UpdateNutritionGoals _endpoint;

    public UpdateNutritionGoalsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new UpdateNutritionGoals(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var request = new UpdateNutritionGoalsRequest(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 65,
            Force: false
        );

        _sender
            .Send(Arg.Any<UpdateNutritionGoalsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateNutritionGoalsCommand>(c =>
                    c.Calories == request.Calories
                    && c.Protein == request.Protein
                    && c.Carbohydrates == request.Carbohydrates
                    && c.Fats == request.Fats
                    && c.Force == request.Force),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenForceIsNull_ShouldDefaultToFalse()
    {
        // Arrange
        var request = new UpdateNutritionGoalsRequest(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 65,
            Force: null
        );

        _sender
            .Send(Arg.Any<UpdateNutritionGoalsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateNutritionGoalsCommand>(c => !c.Force),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new UpdateNutritionGoalsRequest(
            Calories: 2000,
            Protein: 150,
            Carbohydrates: 250,
            Fats: 65,
            Force: false
        );

        var error = new Error("Validation", "Invalid nutrition goals");
        _sender
            .Send(Arg.Any<UpdateNutritionGoalsCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
