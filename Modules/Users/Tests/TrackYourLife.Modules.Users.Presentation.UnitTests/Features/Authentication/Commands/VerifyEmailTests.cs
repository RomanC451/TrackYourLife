using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;
using TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Authentication.Commands;

public class VerifyEmailTests
{
    private readonly ISender _sender;
    private readonly VerifyEmail _endpoint;

    public VerifyEmailTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new VerifyEmail(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var token = "verification-token";
        var request = new VerifyEmailRequest(token);

        _sender
            .Send(Arg.Any<VerifyEmailCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<VerifyEmailCommand>(c => c.VerificationToken == token),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var token = "invalid-token";
        var request = new VerifyEmailRequest(token);

        var error = new Error("Verification", "Invalid or expired token");
        _sender
            .Send(Arg.Any<VerifyEmailCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
