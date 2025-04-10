using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Services;

public class FoodApiAuthDataStoreTests
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        },
        Formatting = Formatting.Indented,
    };

    public class SnakeCaseNamingStrategy : NamingStrategy
    {
        protected override string ResolvePropertyName(string name)
        {
            return string.Concat(
                    name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())
                )
                .ToLower();
        }
    }

    [Fact]
    public void RefreshAuthData_WhenValidJsonResponse_ShouldUpdateAuthDataAndReturnSuccess()
    {
        // Arrange
        var authData = new AuthData
        {
            TokenType = "Bearer",
            AccessToken = "test-access-token",
            ExpiresIn = 3600,
            RefreshToken = "test-refresh-token",
            UserId = "test-user-id",
        };

        string jsonResponse = JsonConvert.SerializeObject(authData, _jsonSerializerSettings);
        var sut = new FoodApiAuthDataStore();

        // Act
        var result = sut.RefreshAuthDataAsync(jsonResponse);

        // Assert
        result.IsSuccess.Should().BeTrue();
        sut.AuthData.Should().NotBeNull();
        sut.AuthData.TokenType.Should().Be(authData.TokenType);
        sut.AuthData.AccessToken.Should().Be(authData.AccessToken);
        sut.AuthData.ExpiresIn.Should().Be(authData.ExpiresIn);
        sut.AuthData.RefreshToken.Should().Be(authData.RefreshToken);
        sut.AuthData.UserId.Should().Be(authData.UserId);
    }

    [Fact]
    public void RefreshAuthData_WhenInvalidJsonResponse_ShouldReturnFailure()
    {
        // Arrange
        string invalidJson = "invalid-json";
        var sut = new FoodApiAuthDataStore();

        // Act
        var result = sut.RefreshAuthDataAsync(invalidJson);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Should()
            .BeEquivalentTo(InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated);
    }

    [Fact]
    public void RefreshAuthData_WhenNullAccessToken_ShouldReturnFailure()
    {
        // Arrange
        var authData = new AuthData
        {
            TokenType = "Bearer",
            AccessToken = string.Empty,
            ExpiresIn = 3600,
            RefreshToken = "test-refresh-token",
            UserId = "test-user-id",
        };

        string jsonResponse = JsonConvert.SerializeObject(authData, _jsonSerializerSettings);
        var sut = new FoodApiAuthDataStore();

        // Act
        var result = sut.RefreshAuthDataAsync(jsonResponse);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Should()
            .BeEquivalentTo(InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated);
    }

    [Fact]
    public void RefreshAuthData_WhenEmptyAccessToken_ShouldReturnFailure()
    {
        // Arrange
        var authData = new AuthData
        {
            TokenType = "Bearer",
            AccessToken = "",
            ExpiresIn = 3600,
            RefreshToken = "test-refresh-token",
            UserId = "test-user-id",
        };

        string jsonResponse = JsonConvert.SerializeObject(authData, _jsonSerializerSettings);
        var sut = new FoodApiAuthDataStore();

        // Act
        var result = sut.RefreshAuthDataAsync(jsonResponse);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Should()
            .BeEquivalentTo(InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated);
    }

    [Fact]
    public void GetAccessToken_ShouldReturnStoredAccessToken()
    {
        // Arrange
        var authData = new AuthData
        {
            TokenType = "Bearer",
            AccessToken = "test-access-token",
            ExpiresIn = 3600,
            RefreshToken = "test-refresh-token",
            UserId = "test-user-id",
        };

        string jsonResponse = JsonConvert.SerializeObject(authData, _jsonSerializerSettings);
        var sut = new FoodApiAuthDataStore();
        sut.RefreshAuthDataAsync(jsonResponse);

        // Act
        string accessToken = sut.GetAccessToken();

        // Assert
        accessToken.Should().Be(authData.AccessToken);
    }

    [Fact]
    public void GetUserId_ShouldReturnStoredUserId()
    {
        // Arrange
        var authData = new AuthData
        {
            TokenType = "Bearer",
            AccessToken = "test-access-token",
            ExpiresIn = 3600,
            RefreshToken = "test-refresh-token",
            UserId = "test-user-id",
        };

        string jsonResponse = JsonConvert.SerializeObject(authData, _jsonSerializerSettings);
        var sut = new FoodApiAuthDataStore();
        sut.RefreshAuthDataAsync(jsonResponse);

        // Act
        string userId = sut.GetUserId();

        // Assert
        userId.Should().Be(authData.UserId);
    }
}
