// using MapsterMapper;
// using MediatR;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.FeatureManagement;
// using Moq;
// using TrackYourLifeDotnet.Application.Abstractions.Services;
// using TrackYourLifeDotnet.Application.Users.Commands.Register;
// using TrackYourLifeDotnet.Domain.Errors;
// using TrackYourLifeDotnet.Domain.Shared;
// using TrackYourLifeDotnet.Domain.Users.StrongTypes;
// using TrackYourLifeDotnet.Presentation.Controllers;

// namespace TrackYourLifeDotnet.Presentation.UnitTests.Controllers.Users;

// public class RegisterUserTests
// {
//     private readonly UserController _sut;
//     private readonly Mock<ISender> _sender = new();

//     private readonly Mock<IAuthService> _authService = new();

//     private readonly Mock<IMapper> _mapper = new();
//     private readonly Mock<IFeatureManager> _featureManager = new();

//     public RegisterUserTests()
//     {
//         _sut = new UserController(
//             _sender.Object,
//             _authService.Object,
//             _mapper.Object,
//             _featureManager.Object
//         );
//     }

//     [Fact]
//     public async void RegisterUser_ReturnsOkResult_WhenCommandSucceeds()
//     {
//         //Arrange
//         RegisterUserRequest request = new("asdasd@asdasd.com", "password", "firstName", "lastName");
//         RegisterUserResult handlerResponse = new(new UserId(Guid.NewGuid()));

//         _sender
//             .Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(Result.Create(handlerResponse));

//         //Act
//         var actionResult = await _sut.RegisterUser(request, CancellationToken.None);

//         //Assert
//         var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
//         var result = Assert.IsType<RegisterUserResponse>(okObjectResult.Value);

//         Assert.Equal(handlerResponse.UserId, result.UserId);
//         _sender.Verify(
//             x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//     }

//     [Fact]
//     public async void RegisterUser_ReturnsBadRequestResult_WhenCommandFails()
//     {
//         //Arrange
//         RegisterUserRequest request = new("", "password", "firstName", "lastName");
//         var handlerResponse = Result.Failure<RegisterUserResult>(DomainErrors.Email.Empty);

//         _sender
//             .Setup(x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync(handlerResponse);

//         //Act
//         var actionResult = await _sut.RegisterUser(request, CancellationToken.None);

//         //Assert
//         var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(actionResult);
//         var badResult = Assert.IsType<ProblemDetails>(badRequestObjectResult.Value);

//         Assert.Equal(badResult.Detail, DomainErrors.Email.Empty.Message);

//         _sender.Verify(
//             x => x.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()),
//             Times.Once
//         );
//     }
// }
