using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RegisterUser;
using TrackYourLife.Modules.Users.Presentation.Features.Authentication.Commands;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Authentication.Commands;

public class RegisterUserTests
{
    private readonly ISender _sender;
    private readonly RegisterUser _endpoint;

    public RegisterUserTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new RegisterUser(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandSucceeds_ShouldReturnCreated()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "test@example.com",
            Password: "Password123!",
            FirstName: "John",
            LastName: "Doe"
        );

        _sender
            .Send(Arg.Any<RegisterUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<Created>();

        await _sender
            .Received(1)
            .Send(
                Arg.Is<RegisterUserCommand>(c =>
                    c.Email == request.Email
                    && c.Password == request.Password
                    && c.FirstName == request.FirstName
                    && c.LastName == request.LastName),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenCommandFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterUserRequest(
            Email: "invalid-email",
            Password: "Password123!",
            FirstName: "John",
            LastName: "Doe"
        );

        var error = new Error("Validation", "Invalid email format");
        _sender
            .Send(Arg.Any<RegisterUserCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
