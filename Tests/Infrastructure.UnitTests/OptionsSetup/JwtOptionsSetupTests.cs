using Microsoft.Extensions.Configuration;
using TrackYourLifeDotnet.Infrastructure.Authentication;
using TrackYourLifeDotnet.Infrastructure.OptionsSetup;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.OptionsSetup;

public class JwtOptionsSetupTests
{
    private JwtOptionsSetup _sut = null!;

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
                    new KeyValuePair<string, string>("Jwt:SecretKey", "testsecretkey"),
                    new KeyValuePair<string, string>("Jwt:MinutesToExpire", "10")
                } as IEnumerable<KeyValuePair<string, string?>>
            )
            .Build();

        _sut = new JwtOptionsSetup(configuration);
        var options = new JwtOptions();

        //Act
        _sut.Configure(options);

        //Assert
        Assert.Equal(configuration["Jwt:Issuer"], options.Issuer);
        Assert.Equal(configuration["Jwt:audience"], options.Audience);
        Assert.Equal(configuration["Jwt:SecretKey"], options.SecretKey);
        Assert.Equal(int.Parse(configuration["Jwt:MinutesToExpire"]!), options.MinutesToExpire);
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
        _sut = new JwtOptionsSetup(configuration);

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => _sut.Configure(options));
    }
}
