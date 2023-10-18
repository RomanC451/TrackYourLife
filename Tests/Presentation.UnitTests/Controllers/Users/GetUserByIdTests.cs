using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Queries;
using TrackYourLifeDotnet.Application.Users.Queries.GetUserById;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Presentation.Controllers;
using Xunit;

namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

public class GetUserByIdTests
{
    private readonly UsersController _sut;
    private readonly Mock<ISender> _sender = new();
    private readonly Mock<IAuthService> _authService = new();

    public GetUserByIdTests()
    {
        _sut = new UsersController(_sender.Object, _authService.Object);
    }

    [Fact]
    public async void GetUserById_ReturnsOk_WhenCommandSucceeds()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        var handlerResponse = new GetUserResponse(
            userId,
            "example@email.com",
            "First Name",
            "Last Name"
        );

        _sender
            .Setup(x => x.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        //Act
        var actionResult = await _sut.GetUserById(userId, CancellationToken.None);

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var controllerResult = Assert.IsType<GetUserResponse>(okObjectResult.Value);

        Assert.Equal(handlerResponse.Id, controllerResult.Id);
        Assert.Equal(handlerResponse.Email, controllerResult.Email);
        Assert.Equal(handlerResponse.FirstName, controllerResult.FirstName);
        Assert.Equal(handlerResponse.LastName, controllerResult.LastName);

        _sender.Verify(
            x => x.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async void GetUserById_ReturnsBadRequest_WhenCommandFails()
    {
        //Arrange
        Guid userId = Guid.NewGuid();
        var handlerResponse = Result.Failure<GetUserResponse>(DomainErrors.User.NotFound(userId));

        _sender
            .Setup(x => x.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResponse);

        //Act
        var actionResult = await _sut.GetUserById(userId, CancellationToken.None);

        //Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var controllerResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        Assert.Equal(handlerResponse.Error.Message, controllerResult.Detail);

        _sender.Verify(
            x => x.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
