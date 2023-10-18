using TrackYourLifeDotnet.Infrastructure.utils;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.Authentication;

public class TokenProviderTests
{
    [Fact]
    public void Generate_Returns_64ByteString()
    {
        // Act
        string refreshToken = TokenProvider.Generate();

        // Assert
        Assert.Equal(64, Convert.FromBase64String(refreshToken).Length);
    }

    [Fact]
    public void Generate_Returns_DifferentValues()
    {
        // Act
        string refreshToken1 = TokenProvider.Generate();
        string refreshToken2 = TokenProvider.Generate();

        // Assert
        Assert.NotEqual(refreshToken1, refreshToken2);
    }
}
