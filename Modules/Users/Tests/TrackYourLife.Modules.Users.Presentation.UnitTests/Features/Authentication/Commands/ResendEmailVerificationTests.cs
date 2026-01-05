using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;
using TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Authentication.Commands;

public class ResendEmailVerificationTests
{
    private readonly ISender _sender;
    private readonly ResendEmailVerification _endpoint;

    public ResendEmailVerificationTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new ResendEmailVerification(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var email = "test@example.com";
        var request = new ResendEmailVerificationRequest(email);

        _sender
            .Send(Arg.Any<ResendEmailVerificationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<ResendEmailVerificationCommand>(c => c.Email == email),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var request = new ResendEmailVerificationRequest(email);

        var error = new Error("User", "User not found");
        _sender
            .Send(Arg.Any<ResendEmailVerificationCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
