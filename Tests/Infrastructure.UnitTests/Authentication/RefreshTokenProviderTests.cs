using TrackYourLifeDotnet.Infrastructure.Authentication;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.Authentication;

public class RefreshTokenProviderTests
{
    private readonly RefreshTokenProvider _refreshTokenProvider;

    public RefreshTokenProviderTests()
    {
        _refreshTokenProvider = new RefreshTokenProvider();
    }

    [Fact]
    public void Generate_Returns_64ByteString()
    {
        // Act
        string refreshToken = _refreshTokenProvider.Generate();

        // Assert
        Assert.Equal(64, Convert.FromBase64String(refreshToken).Length);
    }

    [Fact]
    public void Generate_Returns_DifferentValues()
    {
        // Act
        string refreshToken1 = _refreshTokenProvider.Generate();
        string refreshToken2 = _refreshTokenProvider.Generate();

        // Assert
        Assert.NotEqual(refreshToken1, refreshToken2);
    }
}
