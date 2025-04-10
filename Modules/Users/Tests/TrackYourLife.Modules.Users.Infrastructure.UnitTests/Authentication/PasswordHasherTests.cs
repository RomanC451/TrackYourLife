using TrackYourLife.Modules.Users.Infrastructure.Authentication;

namespace TrackYourLife.Modules.Users.Infrastructure.Tests.Authentication;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void Hash_ValidPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var result = _passwordHasher.Hash(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.Contains(";", result.Value);
    }

    [Fact]
    public void Hash_EmptyPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = string.Empty;

        // Act
        var result = _passwordHasher.Hash(password);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);
        Assert.Contains(";", result.Value);
    }

    [Fact]
    public void Verify_CorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashedPassword = _passwordHasher.Hash(password);

        // Act
        var result = _passwordHasher.Verify(hashedPassword.Value, password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Verify_IncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword123!";
        var hashedPassword = _passwordHasher.Hash(password);

        // Act
        var result = _passwordHasher.Verify(hashedPassword.Value, wrongPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Verify_InvalidHashFormat_ShouldReturnFalse()
    {
        // Arrange
        var invalidHash = "invalidhashformat";
        var password = "TestPassword123!";

        // Act
        var result = _passwordHasher.Verify(invalidHash, password);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Verify_EmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashedPassword = _passwordHasher.Hash(password);

        // Act
        var result = _passwordHasher.Verify(hashedPassword.Value, string.Empty);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Hash_SamePassword_ShouldGenerateDifferentHashes()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hash1 = _passwordHasher.Hash(password);
        var hash2 = _passwordHasher.Hash(password);

        // Assert
        Assert.NotEqual(hash1.Value, hash2.Value);
    }
}
