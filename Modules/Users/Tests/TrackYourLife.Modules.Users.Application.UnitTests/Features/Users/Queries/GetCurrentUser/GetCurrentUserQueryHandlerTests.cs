using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandlerTests
{
    private readonly IUserQuery _userQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _userQuery = Substitute.For<IUserQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetCurrentUserQueryHandler(_userQuery, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ReturnsUser()
    {
        // Arrange
        var userId = UserId.NewId();
        var user = UserFaker.GenerateReadModel(id: userId);
        var query = new GetCurrentUserQuery();

        _userIdentifierProvider.UserId.Returns(userId);
        _userQuery.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var query = new GetCurrentUserQuery();

        _userIdentifierProvider.UserId.Returns(userId);
        _userQuery.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns((UserReadModel?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(UserErrors.NotFound(userId));
    }
}
