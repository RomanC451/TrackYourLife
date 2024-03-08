using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Commands.Remove;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Presentation.Controllers;

namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

public class RemoveUserTests
{
    private readonly UserController _sut;
    private readonly Mock<ISender> _sender = new();
    private readonly Mock<HttpContext> _httpContext = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IMapper> _mapper = new();

    public RemoveUserTests()
    {
        _sut = new UserController(_sender.Object, _authService.Object, _mapper.Object);
    }

    [Fact]
    public async void RemoveUser_RetursOkResult_WhenCommandSucceeds()
    {
        //Arrange
        const string jwtToken = "SuperSecretJwtToken";
        _authService.Setup(x => x.GetHttpContextJwtToken()).Returns(Result.Create(jwtToken));

        UserId deletedUserId = new(Guid.NewGuid());

        _httpContext
            .SetupGet(x => x.Request.Headers["Authorization"])
            .Returns($"jwtTojen {jwtToken}");
        _sut.ControllerContext.HttpContext = _httpContext.Object;

        _sender.Setup(x => x.Send(It.IsAny<RemoveUserCommand>(), It.IsAny<CancellationToken>()));
        //Act
        var actionResult = await _sut.RemoveUser();

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);

        _sender.Verify(
            x => x.Send(It.IsAny<RemoveUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _authService.Verify(x => x.GetHttpContextJwtToken(), Times.Once);
    }

    [Fact]
    public async void RemoveUser_ReturnsBadRequest_WhenCommandFails()
    {
        //Arrange
        const string jwtToken = "SuperSecretJwtToken";
        _authService.Setup(x => x.GetHttpContextJwtToken()).Returns(Result.Create(jwtToken));

        var response = Result.Failure(DomainErrors.JwtToken.Invalid);
        _sender
            .Setup(x => x.Send(It.IsAny<RemoveUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        //Act
        var actionResult = await _sut.RemoveUser();

        //Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var badResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        Assert.Equal(badResult.Detail, DomainErrors.JwtToken.Invalid.Message);
    }
}
