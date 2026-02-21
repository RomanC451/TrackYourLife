using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Presentation.Features.Users.Queries;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Users.Queries;

public class GetCurrentUserTests
{
    private readonly ISender _sender;
    private readonly GetCurrentUser _endpoint;

    public GetCurrentUserTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetCurrentUser(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithUserDto()
    {
        // Arrange
        var userId = UserId.NewId();
        var userReadModel = new UserReadModel(
            Id: userId,
            FirstName: "John",
            LastName: "Doe",
            Email: "john.doe@example.com",
            PasswordHash: "hashed-password",
            VerifiedOnUtc: DateTime.UtcNow,
            PlanType: PlanType.Free,
            StripeCustomerId: null,
            SubscriptionEndsAtUtc: null,
            SubscriptionStatus: null
        );

        _sender
            .Send(Arg.Any<GetCurrentUserQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(userReadModel)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<UserDto>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Id.Should().Be(userId);
        okResult.Value.Email.Should().Be("john.doe@example.com");
        okResult.Value.FirstName.Should().Be("John");
        okResult.Value.LastName.Should().Be("Doe");

        await _sender
            .Received(1)
            .Send(Arg.Any<GetCurrentUserQuery>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnBadRequest()
    {
        // Arrange
        var error = new Error("User", "User not found");
        _sender
            .Send(Arg.Any<GetCurrentUserQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<UserReadModel>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
