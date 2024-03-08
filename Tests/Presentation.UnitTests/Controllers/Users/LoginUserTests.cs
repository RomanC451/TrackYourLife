using TrackYourLifeDotnet.Presentation.Controllers;
using MediatR;
using Moq;
using TrackYourLifeDotnet.Application.Users.Commands.Login;
using TrackYourLifeDotnet.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Domain.Users;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using MapsterMapper;

namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

public class LoginUserTests
{
    private readonly UserController _sut;
    private readonly Mock<ISender> _sender = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IMapper> _mapper = new();

    public LoginUserTests()
    {
        _sut = new UserController(_sender.Object, _authService.Object, _mapper.Object);
    }

    [Fact]
    public async Task LoginUser_ReturnsOkResult_WhenCommandSucceeds()
    {
        // Arrange
        var request = new LoginUserRequest("user@example.com", "password");
        var refreshToken = new UserToken(
            new UserTokenId(Guid.NewGuid()),
            "refreshToken",
            new UserId(Guid.NewGuid()),
            UserTokenTypes.RefreshToken
        );
        var handlerResult = new LoginUserResult(new UserId(Guid.NewGuid()), "jwtToken");
        _sender
            .Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Create(handlerResult));

        // Act
        var actionResult = await _sut.LoginUser(request, CancellationToken.None);

        // Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<LoginUserResponse>(okObjectResult.Value);

        Assert.Equal(handlerResult.UserId, result.UserId);
        Assert.Equal(handlerResult.JwtToken, result.JwtToken);

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
        var handlerResult = Result.Failure<LoginUserResult>(DomainErrors.User.InvalidCredentials);
        _sender
            .Setup(x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResult);

        // Act
        var actionResult = await _sut.LoginUser(request, CancellationToken.None);

        // Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var badResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        Assert.Equal(handlerResult.Error.Message, badResult.Detail);

        _sender.Verify(
            x => x.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
