using FluentAssertions;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Features.Tokens;

public class TokenTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateToken()
    {
        // Arrange
        var id = TokenId.NewId();
        var value = "token123";
        var userId = UserId.NewId();
        var type = TokenType.RefreshToken;
        var expiresAt = DateTime.UtcNow.AddDays(1);
        var deviceId = DeviceId.NewId();

        // Act
        var result = Token.Create(id, value, userId, type, expiresAt, deviceId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(id);
        result.Value.Value.Should().Be(value);
        result.Value.UserId.Should().Be(userId);
        result.Value.Type.Should().Be(type);
        result.Value.ExpiresAt.Should().Be(expiresAt);
        result.Value.DeviceId.Should().Be(deviceId);
        result.Value.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_WithEmptyValue_ShouldFail()
    {
        // Arrange
        var id = TokenId.NewId();
        var value = string.Empty;
        var userId = UserId.NewId();
        var type = TokenType.RefreshToken;
        var expiresAt = DateTime.UtcNow.AddDays(1);

        // Act
        var result = Token.Create(id, value, userId, type, expiresAt);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithExpiredDate_ShouldFail()
    {
        // Arrange
        var id = TokenId.NewId();
        var value = "token123";
        var userId = UserId.NewId();
        var type = TokenType.RefreshToken;
        var expiresAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = Token.Create(id, value, userId, type, expiresAt);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateValue_WithValidValue_ShouldUpdateValue()
    {
        // Arrange
        var token = TokenFaker.Generate();
        var newValue = "newtoken123";

        // Act
        var result = token.UpdateValue(newValue);

        // Assert
        result.IsSuccess.Should().BeTrue();
        token.Value.Should().Be(newValue);
    }

    [Fact]
    public void UpdateValue_WithEmptyValue_ShouldFail()
    {
        // Arrange
        var token = TokenFaker.Generate();
        var newValue = string.Empty;

        // Act
        var result = token.UpdateValue(newValue);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateExpiresAt_WithValidDate_ShouldUpdateExpiresAt()
    {
        // Arrange
        var token = TokenFaker.Generate();
        var newExpiresAt = DateTime.UtcNow.AddDays(2);

        // Act
        var result = token.UpdateExpiresAt(newExpiresAt);

        // Assert
        result.IsSuccess.Should().BeTrue();
        token.ExpiresAt.Should().Be(newExpiresAt);
    }

    [Fact]
    public void UpdateExpiresAt_WithExpiredDate_ShouldFail()
    {
        // Arrange
        var token = TokenFaker.Generate();
        var newExpiresAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var result = token.UpdateExpiresAt(newExpiresAt);

        // Assert
        result.IsFailure.Should().BeTrue();
    }
}
