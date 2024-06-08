using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using TrackYourLife.Common.Infrastructure.Options;
using TrackYourLife.Common.Domain.ServingSizes;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Application.Core.Abstractions.Services;

namespace TrackYourLife.Common.Infrastructure.Services
{
    public class FoodApiService : IFoodApiService
    {
        private readonly FoodApiOptions _foodApiOptions;
        private readonly HttpClient _client;

        private readonly JsonSerializerSettings jsonSerializerSettings =
            new()
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

        public FoodApiService(IOptions<FoodApiOptions> foodApiOptions, HttpClient httpClient)
        {
            _foodApiOptions = foodApiOptions.Value;
            _client = httpClient;
        }

        private async Task<Result> GetAuthData()
        {
            Uri url = new(_foodApiOptions.BaseUrl + _foodApiOptions.AuthTokenPath);

            var response = await _client.GetAsync(url);

            if (response.Content.Headers.ContentType?.MediaType != "application/json")
            {
                return Result.Failure(InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated);
            }

            return Result.Success();
        }

        public async Task<Result<List<Food>>> GetFoodListAsync(
            string foodName,
            int page,
            CancellationToken cancellationToken
        )
        {
            if (page <= 0)
                page = 1;

            var authResult = await GetAuthData();

            if (authResult.IsFailure)
            {
                return Result.Failure<List<Food>>(authResult.Error);
            }

            return await GetFoodsFromAPI(foodName, page, cancellationToken);
        }

        public async Task<Result<List<Food>>> GetFoodsFromAPI(
            string foodName,
            int page,
            CancellationToken cancellationToken
        )
        {
            List<Food> foodList = new();

            var url = new Uri(
                $"{_foodApiOptions.BaseApiUrl}{_foodApiOptions.SearchPath}?query={foodName.Replace(" ", _foodApiOptions.SpaceEncoded)}&page={page}"
            );

            string responseString = await _client.GetStringAsync(url, cancellationToken);

            if (responseString.IsNullOrEmpty())
            {
                return foodList;
            }

            try
            {
                RootObject? rootObject = JsonConvert.DeserializeObject<RootObject>(
                    responseString,
                    jsonSerializerSettings
                );

                if (rootObject is null || rootObject.Items.IsNullOrEmpty())
                    return Result.Failure<List<Food>>(
                        InfrastructureErrors.FoodApiService.FoodNotFound
                    );

                return Result.Success(ConvertToFoodList(rootObject));
            }
            catch (JsonReaderException)
            {
                return Result.Failure<List<Food>>(
                    InfrastructureErrors.FoodApiService.FiledJsonDeserialization
                );
            }
        }

        private static List<Food> ConvertToFoodList(RootObject rootObject)
        {
            List<ApiFood> apiFoods = rootObject.Items.Select(item => item.Food).ToList();

            List<Food> foodList = apiFoods
                .Select(apiFood =>
                {
                    List<FoodServingSize> foodServingSizes = new();

                    var foodId = new FoodId(Guid.NewGuid());

                    for (int i = 0; i < apiFood.ServingSizes.Count; i++)
                    {
                        ServingSize servingSize =
                            new(
                                new ServingSizeId(Guid.NewGuid()),
                                apiFood.ServingSizes[i].NutritionMultiplier,
                                apiFood.ServingSizes[i].Unit,
                                apiFood.ServingSizes[i].Value,
                                apiFood.ServingSizes[i].Id
                            );

                        var foodServingSize = new FoodServingSize(
                            foodId,
                            servingSize.Id,
                            servingSize,
                            i
                        );

                        foodServingSizes.Add(foodServingSize);
                    }

                    return new Food(
                        foodId,
                        apiFood.Type,
                        apiFood.BrandName,
                        apiFood.CountryCode,
                        apiFood.Name,
                        apiFood.NutritionalContent,
                        foodServingSizes,
                        apiFood.Id
                    );
                })
                .ToList();

            return foodList;
        }

        private sealed class ItemListElement
        {
            public ItemListElement(ApiFood food)
            {
                Food = food;
            }

            [JsonProperty("item")]
            public ApiFood Food { get; set; }
        }

        private sealed class RootObject
        {
            [JsonProperty("items")]
            public List<ItemListElement> Items { get; set; } = new();
        }

        private sealed class ApiFood : Food
        {
            public ApiFood(
                long id,
                string type,
                string brandName,
                string countryCode,
                string name,
                NutritionalContent nutritionalContents,
                ICollection<FoodServingSize> foodServingSizes,
                long? apiId
            )
                : base(
                    FoodId.NewId(),
                    type,
                    brandName,
                    countryCode,
                    name,
                    nutritionalContents,
                    foodServingSizes,
                    apiId
                )
            {
                Type = type;
                BrandName = brandName;
                CountryCode = countryCode;
                Name = name;
                NutritionalContent = nutritionalContents;
                FoodServingSizes = foodServingSizes;
                ApiId = apiId;
                Id = id;
            }

            public List<ApiServingSize> ServingSizes { get; set; } = new List<ApiServingSize>();
            public new long Id { get; set; }
        }

        private sealed class ApiServingSize : ServingSize
        {
            public ApiServingSize(
                long id,
                double nutritionMultiplier,
                string unit,
                double value,
                long? apiId
            )
                : base(ServingSizeId.NewId(), nutritionMultiplier, unit, value, apiId)
            {
                Id = id;
                NutritionMultiplier = nutritionMultiplier;
                Unit = unit;
                Value = value;
                ApiId = apiId;
            }

            public new long Id { get; set; }
        }
    }
}
