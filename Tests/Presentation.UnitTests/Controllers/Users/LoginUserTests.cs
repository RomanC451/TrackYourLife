using TrackYourLifeDotnet.Presentation.Controllers;
using MediatR;
using Moq;
using TrackYourLifeDotnet.Application.Users.Commands.Login;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using TrackYourLifeDotnet.Presentation.ControllersResponses.Users;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;

namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

public class LoginUserTests
{
    private readonly UsersController _sut;
    private readonly Mock<ISender> _sender = new();
    private readonly Mock<IAuthService> _authService = new();

    public LoginUserTests()
    {
        _sut = new UsersController(_sender.Object, _authService.Object);
    }

    [Fact]
    public async Task LoginUser_ReturnsOkResult_WhenCommandSucceeds()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "password");
        var refreshToken = new RefreshToken(Guid.NewGuid(), "refreshToken", Guid.NewGuid());
        var response = new LoginUserResponse(Guid.NewGuid(), "jwtToken", refreshToken);
        _sender
            .Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Create(response));

        // Act
        var actionResult = await _sut.LoginUser(request, CancellationToken.None);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<LoginUserControllerResponse>(okObjectResult.Value);

        Assert.Equal(response.UserId, result.UserId);
        Assert.Equal(response.JwtToken, result.JwtToken);

        _sender.Verify(
            x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async Task LoginUser_ReturnsBadRequestResult_WhenCommandFails()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "password");
        var result = Result.Failure<LoginUserResponse>(DomainErrors.User.InvalidCredentials);
        _sender
            .Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        // Act
        var actionResult = await _sut.LoginUser(request, CancellationToken.None);

        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var badResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        Assert.Equal(result.Error.Message, badResult.Detail);

        _sender.Verify(
            x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
