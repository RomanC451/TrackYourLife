using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogOutUser;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Authentication.Commands;

public class LogOutUserTests
{
    private readonly ISender _sender;
    private readonly IAuthCookiesManager _authCookiesManager;
    private readonly LogOutUser _endpoint;

    public LogOutUserTests()
    {
        _sender = Substitute.For<ISender>();
        _authCookiesManager = Substitute.For<IAuthCookiesManager>();
        _endpoint = new LogOutUser(_authCookiesManager, _sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnNoContent()
    {
        // Arrange
        var deviceId = new DeviceId(Guid.NewGuid());
        var logOutAllDevices = false;
        var request = new LogOutUserRequest(deviceId, logOutAllDevices);

        _sender
            .Send(Arg.Any<LogOutUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<NoContent>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<LogOutUserCommand>(c =>
                    c.DeviceId == deviceId && c.LogOutAllDevices == logOutAllDevices
                ),
                Arg.Any<CancellationToken>()
            );

        _authCookiesManager.Received(1).DeleteRefreshTokenCookie();
    }

    [Fact]
    public async Task ExecuteAsync_WhenLogOutAllDevices_ShouldPassCorrectParameter()
    {
        // Arrange
        var deviceId = new DeviceId(Guid.NewGuid());
        var logOutAllDevices = true;
        var request = new LogOutUserRequest(deviceId, logOutAllDevices);

        _sender
            .Send(Arg.Any<LogOutUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        await _sender
            .Received(1)
            .Send(
                Arg.Is<LogOutUserCommand>(c => c.DeviceId == deviceId && c.LogOutAllDevices),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var deviceId = new DeviceId(Guid.NewGuid());
        var request = new LogOutUserRequest(deviceId, false);

        var error = new Error("Logout", "Failed to logout");
        _sender
            .Send(Arg.Any<LogOutUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();

        _authCookiesManager.DidNotReceive().DeleteRefreshTokenCookie();
    }
}
