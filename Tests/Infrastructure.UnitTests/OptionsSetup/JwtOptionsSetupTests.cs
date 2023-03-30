using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using TrackYourLifeDotnet.Infrastructure.Authentication;
using TrackYourLifeDotnet.Infrastructure.OptionsSetup;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.OptionsSetup;

public class JwtOptionsSetupTests
{
    [Fact]
    public void Configure_ConfiguresOptionsCorrectly()
    {
        //Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string>("Jwt:Issuer", "testissuer"),
                    new KeyValuePair<string, string>("Jwt:Audience", "testaudience"),
                    new KeyValuePair<string, string>("Jwt:SecretKey", "testsecretkey")
                } as IEnumerable<KeyValuePair<string, string?>>
            )
            .Build();

        var _sut = new JwtOptionsSetup(configuration);
        var options = new JwtOptions();

        //Act
        _sut.Configure(options);

        //Assert
        Assert.Equal("testissuer", options.Issuer);
        Assert.Equal("testaudience", options.Audience);
        Assert.Equal("testsecretkey", options.SecretKey);
    }

    [Fact]
    public void Configure_WithMissingValues_ThrowsException()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new[]
                {
                    new KeyValuePair<string, string>("Jwt:Issuer", "testissuer"),
                    new KeyValuePair<string, string>("Jwt:SecretKey", "testsecretkey")
                } as IEnumerable<KeyValuePair<string, string?>>
            )
            .Build();
        var options = new JwtOptions();
        var sut = new JwtOptionsSetup(configuration);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => sut.Configure(options));
    }
}
