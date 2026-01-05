using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.DeleteCurrentUser;
using TrackYourLife.Modules.Users.Presentation.Features.Users.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Users.Commands;

public class DeleteCurrentUserTests
{
    private readonly ISender _sender;
    private readonly DeleteUser _endpoint;

    public DeleteCurrentUserTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new DeleteUser(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        _sender
            .Send(Arg.Any<RemoveCurrentUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(Arg.Any<RemoveCurrentUserCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var error = new Error("User", "User not found");
        _sender
            .Send(Arg.Any<RemoveCurrentUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
