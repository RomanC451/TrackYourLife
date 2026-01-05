using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateCurrentUser;
using TrackYourLife.Modules.Users.Presentation.Features.Users.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Users.Commands;

public class UpdateCurrentUserTests
{
    private readonly ISender _sender;
    private readonly UpdateCurrentUser _endpoint;

    public UpdateCurrentUserTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new UpdateCurrentUser(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var request = new UpdateCurrentUserRequest(
            FirstName: "John",
            LastName: "Doe"
        );

        _sender
            .Send(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<UpdateUserCommand>(c =>
                    c.FirstName == request.FirstName && c.LastName == request.LastName),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new UpdateCurrentUserRequest(
            FirstName: "John",
            LastName: "Doe"
        );

        var error = new Error("Validation", "Invalid name");
        _sender
            .Send(Arg.Any<UpdateUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
