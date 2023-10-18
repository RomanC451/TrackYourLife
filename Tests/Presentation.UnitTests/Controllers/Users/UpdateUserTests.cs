using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Commands.Update;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Presentation.Controllers;
using Xunit;

namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

public class UpdateUserTests
{
    private readonly UsersController _sut;
    private readonly Mock<ISender> _sender = new();
    private readonly Mock<IAuthService> _authService = new();

    public UpdateUserTests()
    {
        _sut = new UsersController(_sender.Object, _authService.Object);
    }

    [Fact]
    public async void UpdateUser_ReturnsOk_WhenCommandSucceeds()
    {
        //Arrange
        const string jwtToken = "jwtToken";
        _authService.Setup(x => x.GetHttpContextJwtToken()).Returns(Result.Create(jwtToken));

        var request = new UpdateUserRequest("Joe", "Doe");
        var command = new UpdateUserCommand(jwtToken, request.FirstName, request.LastName);
        var handlerResponse = new UpdateUserResponse(
            Guid.NewGuid(),
            "example@email.com",
            request.FirstName,
            request.LastName
        );

        _sender
            .Setup(x => x.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        //Act
        var controllerResponse = await _sut.UpdateUser(request, CancellationToken.None);

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(controllerResponse);
        var controllerResult = Assert.IsType<UpdateUserResponse>(okObjectResult.Value);

        Assert.Equal(handlerResponse.Id, controllerResult.Id);
        Assert.Equal(handlerResponse.Email, controllerResult.Email);
        Assert.Equal(handlerResponse.FirstName, controllerResult.FirstName);
        Assert.Equal(handlerResponse.LastName, controllerResult.LastName);

        _sender.Verify(
            x => x.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _authService.Verify(x => x.GetHttpContextJwtToken(), Times.Once);
    }

    [Fact]
    public async void UpdateUser_ReturnBadRequest_WhenCommandFails()
    {
        //Arrange
        const string jwtToken = "jwtToken";
        _authService.Setup(x => x.GetHttpContextJwtToken()).Returns(Result.Create(jwtToken));

        var request = new UpdateUserRequest("Joe", "Doe");
        var command = new UpdateUserCommand(jwtToken, request.FirstName, request.LastName);
        var handlerResponse = Result.Failure<UpdateUserResponse>(DomainErrors.Name.TooLong);

        _sender
            .Setup(x => x.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        //Act
        var controllerResponse = await _sut.UpdateUser(request, CancellationToken.None);

        //Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(controllerResponse);
        var badResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        Assert.Equal(DomainErrors.Name.TooLong.Message, badResult.Detail);

        _sender.Verify(
            x => x.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()),
            Times.Once
        );

        _authService.Verify(x => x.GetHttpContextJwtToken(), Times.Once);
    }
}
