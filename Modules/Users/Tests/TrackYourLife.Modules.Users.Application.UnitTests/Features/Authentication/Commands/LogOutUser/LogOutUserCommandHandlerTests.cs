using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogOutUser;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Authentication.Commands.LogOutUser;

public class LogOutUserCommandHandlerTests
{
    private readonly IAuthService _authService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly LogOutUserCommandHandler _handler;

    public LogOutUserCommandHandlerTests()
    {
        _authService = Substitute.For<IAuthService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new LogOutUserCommandHandler(_authService, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var deviceId = DeviceId.NewId();
        var logOutAllDevices = false;
        var command = new LogOutUserCommand(deviceId, logOutAllDevices);

        _userIdentifierProvider.UserId.Returns(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _authService
            .Received(1)
            .LogOutUserAsync(userId, deviceId, logOutAllDevices, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithLogOutAllDevices_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var deviceId = DeviceId.NewId();
        var logOutAllDevices = true;
        var command = new LogOutUserCommand(deviceId, logOutAllDevices);

        _userIdentifierProvider.UserId.Returns(userId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await _authService
            .Received(1)
            .LogOutUserAsync(userId, deviceId, logOutAllDevices, Arg.Any<CancellationToken>());
    }
}
