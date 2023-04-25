using TrackYourLifeDotnet.Infrastructure.Authentication;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.Authentication;

public class RefreshTokenProviderTests
{
    private readonly RefreshTokenProvider _sut;

    public RefreshTokenProviderTests()
    {
        _sut = new RefreshTokenProvider();
    }

    [Fact]
    public void Generate_Returns_64ByteString()
    {
        // Act
        string refreshToken = _sut.Generate();

        // Assert
        Assert.Equal(64, Convert.FromBase64String(refreshToken).Length);
    }

    [Fact]
    public void Generate_Returns_DifferentValues()
    {
        // Act
        string refreshToken1 = _sut.Generate();
        string refreshToken2 = _sut.Generate();

        // Assert
        Assert.NotEqual(refreshToken1, refreshToken2);
    }
}
