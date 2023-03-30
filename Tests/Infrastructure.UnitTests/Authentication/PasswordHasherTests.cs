using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Infrastructure.Authentication;
using Xunit;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.Authentication;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _passwordHasher;

    public PasswordHasherTests()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void Hash_GeneratesNonEmptyString()
    {
        // Arrange
        const string password = "testpassword";

        // Act
        var hash = _passwordHasher.Hash(password);

        // Assert
        Assert.False(string.IsNullOrEmpty(hash));
    }

    [Fact]
    public void Verify_ReturnsTrueForValidPassword()
    {
        // Arrange
        const string password = "testpassword";
        var hash = _passwordHasher.Hash(password);

        // Act
        var result = _passwordHasher.Verify(hash, password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForInvalidPassword()
    {
        // Arrange
        const string password = "testpassword";
        var hash = _passwordHasher.Hash(password);

        // Act
        var result = _passwordHasher.Verify(hash, "invalidpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForInvalidHash()
    {
        // Arrange
        const string password = "testpassword";
        var invalidHash = _passwordHasher.Hash("testpassword2");

        // Act
        var result = _passwordHasher.Verify(invalidHash, password);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForInvalidHashFormat()
    {
        // Arrange
        const string password = "testpassword";
        const string invalidHash = "invalidhashformat";

        // Act
        var result = _passwordHasher.Verify(invalidHash, password);

        // Assert
        Assert.False(result);
    }
}
