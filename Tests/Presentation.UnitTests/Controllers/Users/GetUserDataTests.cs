using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Services;
using TrackYourLifeDotnet.Application.Users.Queries;
using TrackYourLifeDotnet.Application.Users.Queries.GetUserData;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Shared;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;
using TrackYourLifeDotnet.Presentation.Controllers;
using Xunit;

namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

public class GetUserDataTests
{
    private readonly UserController _sut;
    private readonly Mock<ISender> _sender = new();
    private readonly Mock<IAuthService> _authService = new();
    private readonly Mock<IMapper> _mapper = new();

    public GetUserDataTests()
    {
        _sut = new UserController(_sender.Object, _authService.Object, _mapper.Object);
    }

    [Fact]
    public async void GetUserData_ReturnsOk_WhenCommandSucceeds()
    {
        //Arrange
        UserId userId = new(Guid.NewGuid());
        var handlerResult = new GetUserDataResult(
            userId,
            "example@email.com",
            "First Name",
            "Last Name"
        );

        _sender
            .Setup(x => x.Send(It.IsAny<GetUserDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResult);

        //Act
        var actionResult = await _sut.GetUserData(CancellationToken.None);

        //Assert
        var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
        var controllerResponse = Assert.IsType<GetUserDataResponse>(okObjectResult.Value);

        Assert.Equal(handlerResult.Id, controllerResponse.Id);
        Assert.Equal(handlerResult.Email, controllerResponse.Email);
        Assert.Equal(handlerResult.FirstName, controllerResponse.FirstName);
        Assert.Equal(handlerResult.LastName, controllerResponse.LastName);

        _sender.Verify(
            x => x.Send(It.IsAny<GetUserDataQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Fact]
    public async void GetUserData_ReturnsBadRequest_WhenCommandFails()
    {
        //Arrange
        UserId userId = new(Guid.NewGuid());
        var handlerResult = Result.Failure<GetUserDataResult>(DomainErrors.User.NotFound(userId));

        _sender
            .Setup(x => x.Send(It.IsAny<GetUserDataQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(handlerResult);

        //Act
        var actionResult = await _sut.GetUserData(CancellationToken.None);

        //Assert
        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
        var controllerResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

        Assert.Equal(handlerResult.Error.Message, controllerResult.Detail);

        _sender.Verify(
            x => x.Send(It.IsAny<GetUserDataQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }
}
