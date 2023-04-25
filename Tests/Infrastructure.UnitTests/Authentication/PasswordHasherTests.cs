using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Infrastructure.Authentication;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.Authentication;

public class PasswordHasherTests
{
    private readonly IPasswordHasher _sut;

    public PasswordHasherTests()
    {
        _sut = new PasswordHasher();
    }

    [Fact]
    public void Hash_GeneratesNonEmptyString()
    {
        // Arrange
        const string password = "testpassword";

        // Act
        var hash = _sut.Hash(password);

        // Assert
        Assert.False(string.IsNullOrEmpty(hash.Value));
    }

    [Fact]
    public void Verify_ReturnsTrueForValidPassword()
    {
        // Arrange
        const string password = "testpassword";
        var hash = _sut.Hash(password);

        // Act
        var result = _sut.Verify(hash.Value, password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForInvalidPassword()
    {
        // Arrange
        const string password = "testpassword";
        var hash = _sut.Hash(password);

        // Act
        var result = _sut.Verify(hash.Value, "invalidpassword");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Verify_ReturnsFalseForInvalidHash()
    {
        // Arrange
        const string password = "testpassword";
        var invalidHash = _sut.Hash("testpassword2");

        // Act
        var result = _sut.Verify(invalidHash.Value, password);

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
        var result = _sut.Verify(invalidHash, password);

        // Assert
        Assert.False(result);
    }
}
