using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Users.Commands.RefreshJwtToken;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Presentation.Controllers;

namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

public class RefreshTokenTests
{
    private readonly UsersController _sut;
    private readonly Mock<ISender> _sender = new();
    private readonly Mock<IAuthService> _authService = new();

    public RefreshTokenTests(){
        _sut = new UsersController(_sender.Object, _authService.Object);
    }

    [Fact]
    public async void RefreshToken_ReturnsOk_WhenCommandSucceeds(){
        //Arrange
        const string jwtToken = "jwtToken";
        _authService.Setup(x => x.GetHttpContextJwtToken()).Returns(jwtToken);

        var refreshToken = new RefreshToken(Guid.NewGuid(), "refreshToken", Guid.NewGuid());
        var handlerResponse = new RefreshJwtTokenResponse(jwtToken, refreshToken);

        _sender.Setup(x=> x.Send(It.IsAny<RefreshJwtTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResponse);

        //Act
        var controllerResponse = await _sut.RefreshToken(CancellationToken.None);

        //Asert
        var okObjectResult = Assert.IsType<OkObjectResult>(controllerResponse);
        var responseJwtToken = Assert.IsType<string>(okObjectResult.Value);

        Assert.Equal(jwtToken, responseJwtToken);

        _authService.Verify(x => x.GetHttpContextJwtToken(), Times.Once);
        _sender.Verify(x => x.Send(It.IsAny<RefreshJwtTokenCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void RefreshToken_ReturnsBadRequest_WhenCommandFails(){
        //Arrange
        const string jwtToken = "jwtToken";
        _authService.Setup(x => x.GetHttpContextJwtToken()).Returns(jwtToken);

        var refreshToken = new RefreshToken(Guid.NewGuid(), "refreshToken", Guid.NewGuid());
        var handlerResponse = Result.Failure<RefreshJwtTokenResponse>(DomainErrors.User.InvalidJwtToken);

        _sender.Setup(x => x.Send(It.IsAny<RefreshJwtTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(handlerResponse);

        _authService.Verify(x => x.GetHttpContextJwtToken(), Times.Never);

        //Act
        var controllerResponse = await _sut.RefreshToken(CancellationToken.None);

        //Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(controllerResponse);
        var objectResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        Assert.Equal(DomainErrors.User.InvalidJwtToken.Message, objectResult.Detail);

        _authService.Verify(x => x.GetHttpContextJwtToken(), Times.Once);
        _sender.Verify(x => x.Send(It.IsAny<RefreshJwtTokenCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
