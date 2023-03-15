using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Application.Users.Commands.Register;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Presentation.Controllers;
using TrackYourLifeDotnet.Presentation.ControllersResponses.Users;

namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

public class RegisterUserTests
{
    private readonly UsersController _sut;
    private readonly Mock<ISender> _sender = new();

    private readonly Mock<IAuthService> _authService = new();

    public RegisterUserTests()
    {
        _sut = new UsersController(_sender.Object, _authService.Object);
    }

    [Fact]
    public async void RegisterUser_ReturnsOkResult_WhenCommandSucceeds()
    {
        //Arrange
        RegisterUserRequest request = new("asdasd@asdasd.com", "password", "firstName", "lastName");
        RegisterUserResponse handlerResponse =
            new(
                Guid.NewGuid(),
                "jwtToken",
                new RefreshToken(Guid.NewGuid(), "refreshToken", Guid.NewGuid())
            );

        _sender
            .Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Create(handlerResponse));

        //Act
        var actionResult = await _sut.RegisterUser(request, CancellationToken.None);

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<RegisterUserControllerResponse>(okObjectResult.Value);
        
        Assert.Equal(handlerResponse.UserId, result.UserId);
        Assert.Equal(handlerResponse.JwtToken, result.JwtToken);

        _sender.Verify(
            x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async void RegisterUser_ReturnsBadRequestResult_WhenCommandFails()
    {
        //Arrange
        RegisterUserRequest request = new("", "password", "firstName", "lastName");
        var handlerResponse = Result.Failure<RegisterUserResponse>(DomainErrors.Email.Empty);

        _sender
            .Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        //Act
        var actionResult = await _sut.RegisterUser(request, CancellationToken.None);

        //Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var badResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        Assert.Equal(badResult.Detail, DomainErrors.Email.Empty.Message);

        _sender.Verify(
            x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
