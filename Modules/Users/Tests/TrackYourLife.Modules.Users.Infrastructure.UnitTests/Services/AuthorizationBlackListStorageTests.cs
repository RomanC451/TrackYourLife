using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Services;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Services;

public class AuthorizationBlackListStorageTests
{
    private readonly AuthorizationBlackListStorage _sut;

    public AuthorizationBlackListStorageTests()
    {
        _sut = new AuthorizationBlackListStorage();
    }

    [Fact]
    public void Add_ShouldAddUserToBlackList()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var deviceId = DeviceId.Create(Guid.NewGuid());
        var token = "test-token";
        var user = new LoggedOutUser(token, userId, deviceId);

        // Act
        _sut.Add(user);

        // Assert
        _sut.Contains(token).Should().BeTrue();
    }

    [Fact]
    public void Contains_WhenTokenExists_ShouldReturnTrue()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var deviceId = DeviceId.Create(Guid.NewGuid());
        var token = "test-token";
        var user = new LoggedOutUser(token, userId, deviceId);
        _sut.Add(user);

        // Act
        var result = _sut.Contains(token);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Contains_WhenTokenDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentToken = "non-existent-token";

        // Act
        var result = _sut.Contains(nonExistentToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Remove_ShouldRemoveUserFromBlackList()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var deviceId = DeviceId.Create(Guid.NewGuid());
        var token = "test-token";
        var user = new LoggedOutUser(token, userId, deviceId);
        _sut.Add(user);

        // Act
        _sut.Remove(userId, deviceId);

        // Assert
        _sut.Contains(token).Should().BeFalse();
    }

    [Fact]
    public void Remove_WhenUserDoesNotExist_ShouldNotThrowException()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var deviceId = DeviceId.Create(Guid.NewGuid());

        // Act & Assert
        var act = () => _sut.Remove(userId, deviceId);
        act.Should().NotThrow();
    }

    [Fact]
    public void Add_WhenAddingSameUserTwice_ShouldNotThrowException()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var deviceId = DeviceId.Create(Guid.NewGuid());
        var token = "test-token";
        var user = new LoggedOutUser(token, userId, deviceId);

        // Act & Assert
        var act = () =>
        {
            _sut.Add(user);
            _sut.Add(user);
        };
        act.Should().NotThrow();
    }
}
