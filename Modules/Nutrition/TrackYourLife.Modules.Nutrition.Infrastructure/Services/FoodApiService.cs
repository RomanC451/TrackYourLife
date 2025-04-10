using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Services;

/// <summary>
/// Represents the service for interacting with the food API.
/// </summary>
internal sealed class FoodApiService : IFoodApiService
{
    private const int MaxSearchResults = 20;

    private readonly FoodApiOptions foodApiOptions;
    private readonly JsonSerializerSettings jsonSerializerSettings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        },
    };
    private readonly HttpClient httpClient;
    private readonly IFoodRepository foodRepository;
    private readonly INutritionUnitOfWork unitOfWork;
    private readonly IServingSizeRepository servingSizeRepository;
    private readonly ISearchedFoodRepository searchedFoodRepository;

    private readonly IFoodApiAuthDataStore authDataStore;

    public FoodApiService(
        IOptions<FoodApiOptions> foodApiOptions,
        HttpClient httpClient,
        IFoodRepository foodRepository,
        IServingSizeRepository servingSizeRepository,
        ISearchedFoodRepository searchedFoodRepository,
        INutritionUnitOfWork unitOfWork,
        IFoodApiAuthDataStore authDataStore
    )
    {
        this.servingSizeRepository = servingSizeRepository;
        this.searchedFoodRepository = searchedFoodRepository;
        this.foodApiOptions = foodApiOptions.Value;
        this.httpClient = httpClient;
        this.foodRepository = foodRepository;
        this.unitOfWork = unitOfWork;
        this.authDataStore = authDataStore;

        SetupHttpClientHeaders();
    }

    /// <summary>
    /// Sets up the headers for the HTTP client.
    /// </summary>
    private void SetupHttpClientHeaders()
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                authDataStore.GetAccessToken()
            );

        if (httpClient.DefaultRequestHeaders.Contains("mfp-client-id"))
            httpClient.DefaultRequestHeaders.Remove("mfp-client-id");
        httpClient.DefaultRequestHeaders.Add("mfp-client-id", "mfp-main-js");

        if (httpClient.DefaultRequestHeaders.Contains("mfp-user-id"))
            httpClient.DefaultRequestHeaders.Remove("mfp-user-id");
        httpClient.DefaultRequestHeaders.Add("mfp-user-id", authDataStore.GetUserId());
    }

    /// <summary>
    /// Gets the search URI for the specified food name.
    /// </summary>
    /// <param name="foodName">The name of the food to search for.</param>
    /// <returns>The search URI.</returns>
    private Uri GetSearchUri(string foodName) =>
        new(
            $"{foodApiOptions.BaseApiUrl}{foodApiOptions.SearchPath}?q={foodName.Replace(" ", foodApiOptions.SpaceEncoded)}&max_items={MaxSearchResults}"
        );

    /// <summary>
    /// Refresh the authentication data for the food API.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
    public async Task<Result> GetAuthDataAsync()
    {
        Uri url = new(foodApiOptions.BaseUrl + foodApiOptions.AuthTokenPath);

        var response = await httpClient.GetStringAsync(url);

        var result = authDataStore.RefreshAuthDataAsync(response);

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        SetupHttpClientHeaders();

        return Result.Success();
    }

    /// <summary>
    /// Hits the endpoint of the api for getting the best matches for the food name and adds the foods to the database.
    /// </summary>
    /// <param name="foodName">The name of the food to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task<Result> SearchFoodAndAddToDbAsync(
        string foodName,
        CancellationToken cancellationToken
    )
    {
        var searchResult = await SearchFoodAsync(foodName, cancellationToken);

        if (searchResult.IsFailure)
        {
            return Result.Failure(searchResult.Error);
        }

        var result = SearchedFood.Create(SearchedFoodId.NewId(), foodName.ToLower());

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }
        await searchedFoodRepository.AddAsync(result.Value, cancellationToken);

        if (searchResult.Value.Item1.Count != 0)
        {
            await foodRepository.AddRangeAsync(searchResult.Value.Item1, cancellationToken);
        }

        if (searchResult.Value.Item2.Count != 0)
        {
            await servingSizeRepository.AddRangeAsync(searchResult.Value.Item2, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result<(List<Food>, List<ServingSize>)>> SearchFoodAsync(
        string foodName,
        CancellationToken cancellationToken
    )
    {
        string responseString;
        try
        {
            responseString = await httpClient.GetStringAsync(
                GetSearchUri(foodName),
                cancellationToken
            );
        }
        catch (HttpRequestException)
        {
            try
            {
                var authResult = await GetAuthDataAsync();

                if (authResult.IsFailure)
                {
                    return Result.Failure<(List<Food>, List<ServingSize>)>(authResult.Error);
                }

                responseString = await httpClient.GetStringAsync(
                    GetSearchUri(foodName),
                    cancellationToken
                );
            }
            catch
            {
                return Result.Failure<(List<Food>, List<ServingSize>)>(
                    InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated
                );
            }
        }

        FoodApiResponse? foodApiResponse;

        try
        {
            foodApiResponse = JsonConvert.DeserializeObject<FoodApiResponse>(
                responseString,
                jsonSerializerSettings
            );
        }
        catch (JsonReaderException)
        {
            return Result.Failure<(List<Food>, List<ServingSize>)>(
                InfrastructureErrors.FoodApiService.FailedJsonDeserialization
            );
        }

        if (foodApiResponse is null || foodApiResponse.Items.Count == 0)
            return Result.Failure<(List<Food>, List<ServingSize>)>(
                InfrastructureErrors.FoodApiService.FoodNotFound
            );

        return Result.Success(await ConvertToEntitiesAsync(foodApiResponse, cancellationToken));
    }

    /// <summary>
    /// Converts the API response to a list of food items.
    /// </summary>
    /// <param name="foodApiResponse">The root object containing the API response.</param>
    /// <returns>The list of food items.</returns>
    private async Task<(List<Food>, List<ServingSize>)> ConvertToEntitiesAsync(
        FoodApiResponse foodApiResponse,
        CancellationToken cancellationToken
    )
    {
        List<ApiFood> apiFoods = foodApiResponse
            .Items.Select(item => item.Item)
            .Take(MaxSearchResults)
            .ToList();

        var foodApiIds = apiFoods.Select(f => f.Id).Distinct().ToList();

        var existingFoods = await foodRepository.GetWhereApiIdPartOfListAsync(
            foodApiIds,
            cancellationToken
        );

        var servingSizeApiIds = apiFoods
            .SelectMany(f => f.ServingSizes)
            .Select(ss => ss.Id)
            .Distinct()
            .ToList();

        var existingServingSizes = await servingSizeRepository.GetWhereApiIdPartOfListAsync(
            servingSizeApiIds,
            cancellationToken
        );

        List<ServingSize> servingSizesResponse = [];

        List<Food> foodList = [];
        foreach (var apiFood in apiFoods)
        {
            List<FoodServingSize> foodServingSizes = [];
            List<ServingSize> tmpServingSizesList = [];

            var foodId = FoodId.NewId();

            for (int i = 0; i < apiFood.ServingSizes.Count; i++)
            {
                var ssResult = ServingSize.Create(
                    ServingSizeId.NewId(),
                    apiFood.ServingSizes[i].NutritionMultiplier,
                    apiFood.ServingSizes[i].Unit,
                    apiFood.ServingSizes[i].Value,
                    apiFood.ServingSizes[i].Id
                );

                if (ssResult.IsFailure)
                {
                    continue;
                }

                ServingSize servingSize;

                var existingServingSize = existingServingSizes.Find(ss =>
                    ss.ApiId == apiFood.ServingSizes[i].Id
                );

                if (existingServingSize is not null)
                {
                    servingSize = existingServingSize;
                }
                else
                {
                    servingSize = ssResult.Value;
                    tmpServingSizesList.Add(servingSize);
                }

                var fssResult = FoodServingSize.Create(foodId, servingSize.Id, i);

                if (fssResult.IsFailure)
                {
                    continue;
                }
                foodServingSizes.Add(fssResult.Value);
            }

            if (existingFoods.Exists(f => f.ApiId == apiFood.Id))
            {
                servingSizesResponse.AddRange(tmpServingSizesList);
                continue;
            }

            // Create food even if no serving sizes were created successfully
            var foodResult = Food.Create(
                foodId,
                apiFood.Type,
                apiFood.BrandName,
                apiFood.CountryCode,
                apiFood.Description,
                apiFood.NutritionalContents,
                foodServingSizes,
                apiFood.Id
            );

            if (foodResult.IsFailure)
            {
                continue;
            }

            foodList.Add(foodResult.Value);
            servingSizesResponse.AddRange(tmpServingSizesList);
        }

        return (foodList, servingSizesResponse);
    }
}
