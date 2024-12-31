using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Services;

internal sealed class FoodApiAuthDataStore : IFoodApiAuthDataStore
{
    public AuthData AuthData { get; private set; } = new();

    private readonly JsonSerializerSettings jsonSerializerSettings =
        new()
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

    /// <summary>
    /// Refresh the authentication data for the food API.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
    public Result RefreshAuthDataAsync(string apiResponse)
    {
        try
        {
            AuthData = JsonConvert.DeserializeObject<AuthData>(
                apiResponse,
                jsonSerializerSettings
            )!;
        }
        catch (JsonReaderException)
        {
            return Result.Failure(InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated);
        }

        if (AuthData is null || string.IsNullOrEmpty(AuthData.AccessToken))
        {
            return Result.Failure(InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated);
        }

        return Result.Success();
    }

    public string GetAccessToken()
    {
        return AuthData.AccessToken;
    }

    public string GetUserId()
    {
        return AuthData.UserId;
    }
}
