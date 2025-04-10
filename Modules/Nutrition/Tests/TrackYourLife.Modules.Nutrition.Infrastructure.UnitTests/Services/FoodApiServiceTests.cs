using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RichardSzalay.MockHttp;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services;
using TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Services;

public class FoodApiServiceTests : IDisposable
{
    private readonly IOptions<FoodApiOptions> foodApiOptions =
        Microsoft.Extensions.Options.Options.Create(
            new FoodApiOptions()
            {
                BaseUrl = "https://api.myfitnesspal.com",
                BaseApiUrl = "https://api.myfitnesspal.com",
                AuthTokenPath = "/user/auth_token?refresh=true",
                SearchPath = "/v2/nutrition",
                SpaceEncoded = "%20",
            }
        );
    private readonly IFoodRepository foodRepository = Substitute.For<IFoodRepository>();
    private readonly IServingSizeRepository servingSizeRepository =
        Substitute.For<IServingSizeRepository>();
    private readonly ISearchedFoodRepository searchedFoodRepository =
        Substitute.For<ISearchedFoodRepository>();
    private readonly INutritionUnitOfWork unitOfWork = Substitute.For<INutritionUnitOfWork>();
    private readonly IFoodApiAuthDataStore authDataStore = Substitute.For<IFoodApiAuthDataStore>();

    public FoodApiServiceTests()
    {
        authDataStore.GetAccessToken().Returns("test-token");
        authDataStore.GetUserId().Returns("test-user");
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foodRepository.ClearSubstitute();
        servingSizeRepository.ClearSubstitute();
        searchedFoodRepository.ClearSubstitute();
        unitOfWork.ClearSubstitute();
        authDataStore.ClearSubstitute();
    }

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

    private readonly JsonSerializerSettings jsonOptions = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        },
        Formatting = Formatting.Indented,
    };

    [Fact]
    public async Task SearchFoodAndAddToDb_WhenFailsToAuthenticate_ShouldReturnFailure()
    {
        // Arrange
        var foodName = "laptte";
        var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(authUrl).Respond("application/json", "");

        var searchUrl = "https://api.myfitnesspal.com/v2/nutrition?q=laptte&max_items=20";

        mockHttp.When(searchUrl).Throw(new HttpRequestException());

        var sut = new FoodApiService(
            foodApiOptions,
            new HttpClient(mockHttp),
            foodRepository,
            servingSizeRepository,
            searchedFoodRepository,
            unitOfWork,
            authDataStore
        );

        // Act
        var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Should()
            .BeEquivalentTo(InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated);
        await foodRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());
        await servingSizeRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>());
        await searchedFoodRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchFoodAndAddToDb_WhenFailsToDeserializeTheResponse_ShouldReturnFailure()
    {
        // Arrange
        var foodName = "laptte";
        var authData = new AuthData()
        {
            TokenType = "token",
            AccessToken = "accessToken",
            ExpiresIn = 3600,
            RefreshToken = "refreshToken",
            UserId = "user",
        };

        var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;
        string returnedString = JsonConvert.SerializeObject(authData, jsonOptions);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(authUrl).Respond("application/json", returnedString);

        var searchUrl =
            $"{foodApiOptions.Value.BaseApiUrl}{foodApiOptions.Value.SearchPath}?q={foodName.Replace(" ", foodApiOptions.Value.SpaceEncoded)}";
        mockHttp.When(searchUrl).Respond("application/json", "asd");

        var sut = new FoodApiService(
            foodApiOptions,
            new HttpClient(mockHttp),
            foodRepository,
            servingSizeRepository,
            searchedFoodRepository,
            unitOfWork,
            authDataStore
        );

        // Act
        var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Should()
            .BeEquivalentTo(InfrastructureErrors.FoodApiService.FailedJsonDeserialization);
        await foodRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());
        await servingSizeRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>());
        await searchedFoodRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchFoodAndAddToDb_WhenResponseItemsListIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var foodName = "laptte";
        var authData = new AuthData()
        {
            TokenType = "token",
            AccessToken = "accessToken",
            ExpiresIn = 3600,
            RefreshToken = "refreshToken",
            UserId = "user",
        };

        var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;
        string returnedString = JsonConvert.SerializeObject(authData, jsonOptions);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(authUrl).Respond("application/json", returnedString);

        var searchUrl =
            $"{foodApiOptions.Value.BaseApiUrl}{foodApiOptions.Value.SearchPath}?q={foodName.Replace(" ", foodApiOptions.Value.SpaceEncoded)}";
        var searchResponse = new FoodApiResponse();
        mockHttp
            .When(searchUrl)
            .Respond("application/json", JsonConvert.SerializeObject(searchResponse, jsonOptions));

        var sut = new FoodApiService(
            foodApiOptions,
            new HttpClient(mockHttp),
            foodRepository,
            servingSizeRepository,
            searchedFoodRepository,
            unitOfWork,
            authDataStore
        );

        // Act
        var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(InfrastructureErrors.FoodApiService.FoodNotFound);
        await foodRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());
        await servingSizeRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>());
        await searchedFoodRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchFoodAndAddToDb_WhenResponseIsValid_ShouldAddFoodsAndServingSizesToDbAndReturnSuccess()
    {
        // Arrange
        var foodName = "laptte";
        var authData = new AuthData()
        {
            TokenType = "token",
            AccessToken = "accessToken",
            ExpiresIn = 3600,
            RefreshToken = "refreshToken",
            UserId = "user",
        };

        var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;
        string returnedString = JsonConvert.SerializeObject(authData, jsonOptions);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(authUrl).Respond("application/json", returnedString);

        servingSizeRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<ServingSize>());

        foodRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Food>());

        var searchResponse = new FoodApiResponse()
        {
            Items =
            [
                new ItemListElement()
                {
                    Item = new ApiFood()
                    {
                        Id = 123124123213,
                        Description = "lapte",
                        Type = "food",
                        BrandName = "Test Brand",
                        CountryCode = "US",
                        NutritionalContents = new NutritionalContent
                        {
                            Energy = new Energy { Value = 100, Unit = "Kcal" },
                            Protein = 10,
                            Carbohydrates = 20,
                            Fat = 5,
                        },
                        ServingSizes =
                        [
                            new ApiServingSize()
                            {
                                Id = 123123123,
                                NutritionMultiplier = 1,
                                Unit = "ml",
                                Value = 100,
                            },
                        ],
                    },
                },
            ],
        };

        List<Food> capturedFood = [];
        List<ServingSize> capturedServingSizes = [];
        SearchedFood capturedSearchedFood = null!;

        foodRepository
            .When(x => x.AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedFood = callInfo.Arg<List<Food>>());

        servingSizeRepository
            .When(x => x.AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedServingSizes = callInfo.Arg<List<ServingSize>>());

        searchedFoodRepository
            .When(x => x.AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedSearchedFood = callInfo.Arg<SearchedFood>());

        mockHttp
            .When(HttpMethod.Get, "https://api.myfitnesspal.com/v2/nutrition*")
            .Respond("application/json", JsonConvert.SerializeObject(searchResponse, jsonOptions));

        var sut = new FoodApiService(
            foodApiOptions,
            new HttpClient(mockHttp),
            foodRepository,
            servingSizeRepository,
            searchedFoodRepository,
            unitOfWork,
            authDataStore
        );

        // Act
        var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFood.Should().HaveCount(1);
        capturedFood[0].Name.Should().Be(searchResponse.Items[0].Item.Description);
        capturedFood[0].ApiId.Should().Be(searchResponse.Items[0].Item.Id);
        capturedFood[0].FoodServingSizes.Should().HaveCount(1);
        capturedFood[0].FoodServingSizes.ToList()[0].Index.Should().Be(0);

        capturedServingSizes.Should().HaveCount(1);
        capturedServingSizes[0]
            .NutritionMultiplier.Should()
            .Be(searchResponse.Items[0].Item.ServingSizes[0].NutritionMultiplier);
        capturedServingSizes[0].Unit.Should().Be(searchResponse.Items[0].Item.ServingSizes[0].Unit);
        capturedServingSizes[0]
            .Value.Should()
            .Be(searchResponse.Items[0].Item.ServingSizes[0].Value);

        capturedSearchedFood.Should().NotBeNull();
        capturedSearchedFood.Name.Should().Be(foodName.ToLower());

        await foodRepository
            .Received(1)
            .AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());
        await servingSizeRepository
            .Received(1)
            .AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>());
        await searchedFoodRepository
            .Received(1)
            .AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchFoodAndAddToDb_WhenFoodAlreadyExists_ShouldSkipExistingFood()
    {
        // Arrange
        var foodName = "laptte";
        var authData = new AuthData()
        {
            TokenType = "token",
            AccessToken = "accessToken",
            ExpiresIn = 3600,
            RefreshToken = "refreshToken",
            UserId = "user",
        };

        var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;
        string returnedString = JsonConvert.SerializeObject(authData, jsonOptions);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(authUrl).Respond("application/json", returnedString);

        servingSizeRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<ServingSize>());

        var searchResponse = new FoodApiResponse()
        {
            Items =
            [
                new ItemListElement()
                {
                    Item = new ApiFood()
                    {
                        Id = 123124123213,
                        Description = "lapte",
                        Type = "food",
                        BrandName = "Test Brand",
                        CountryCode = "US",
                        NutritionalContents = new NutritionalContent
                        {
                            Energy = new Energy { Value = 100, Unit = "Kcal" },
                            Protein = 10,
                            Carbohydrates = 20,
                            Fat = 5,
                        },
                        ServingSizes =
                        [
                            new ApiServingSize()
                            {
                                Id = 123123123,
                                NutritionMultiplier = 1,
                                Unit = "ml",
                                Value = 100,
                            },
                        ],
                    },
                },
            ],
        };

        foodRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Food> { FoodFaker.Generate(apiId: 123124123213, name: "lapte") });

        mockHttp
            .When(HttpMethod.Get, "https://api.myfitnesspal.com/v2/nutrition*")
            .Respond("application/json", JsonConvert.SerializeObject(searchResponse, jsonOptions));

        var sut = new FoodApiService(
            foodApiOptions,
            new HttpClient(mockHttp),
            foodRepository,
            servingSizeRepository,
            searchedFoodRepository,
            unitOfWork,
            authDataStore
        );

        // Act
        var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        await foodRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());
        await servingSizeRepository
            .Received(1)
            .AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>());
        await searchedFoodRepository
            .Received(1)
            .AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchFoodAndAddToDb_WhenServingSizeAlreadyExists_ShouldSkipExistingServingSize()
    {
        // Arrange
        var foodName = "laptte";
        var authData = new AuthData()
        {
            TokenType = "token",
            AccessToken = "accessToken",
            ExpiresIn = 3600,
            RefreshToken = "refreshToken",
            UserId = "user",
        };

        var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;
        string returnedString = JsonConvert.SerializeObject(authData, jsonOptions);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(authUrl).Respond("application/json", returnedString);

        foodRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Food>());

        var searchResponse = new FoodApiResponse()
        {
            Items =
            [
                new ItemListElement()
                {
                    Item = new ApiFood()
                    {
                        Id = 123124123213,
                        Description = "lapte",
                        Type = "food",
                        BrandName = "Test Brand",
                        CountryCode = "US",
                        NutritionalContents = new NutritionalContent
                        {
                            Energy = new Energy { Value = 100, Unit = "Kcal" },
                            Protein = 10,
                            Carbohydrates = 20,
                            Fat = 5,
                        },
                        ServingSizes =
                        [
                            new ApiServingSize()
                            {
                                Id = 123123123,
                                NutritionMultiplier = 1,
                                Unit = "ml",
                                Value = 100,
                            },
                        ],
                    },
                },
            ],
        };

        servingSizeRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(
                new List<ServingSize>
                {
                    ServingSize
                        .Create(
                            ServingSizeFaker.GenerateReadModel(apiId: 123123123).Id,
                            1,
                            "ml",
                            100,
                            123123123
                        )
                        .Value,
                }
            );

        List<Food> capturedFood = [];
        List<ServingSize> capturedServingSizes = [];
        SearchedFood capturedSearchedFood = null!;

        foodRepository
            .When(x => x.AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedFood = callInfo.Arg<List<Food>>());

        servingSizeRepository
            .When(x => x.AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedServingSizes = callInfo.Arg<List<ServingSize>>());

        searchedFoodRepository
            .When(x => x.AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedSearchedFood = callInfo.Arg<SearchedFood>());

        mockHttp
            .When(HttpMethod.Get, "https://api.myfitnesspal.com/v2/nutrition*")
            .Respond("application/json", JsonConvert.SerializeObject(searchResponse, jsonOptions));

        var sut = new FoodApiService(
            foodApiOptions,
            new HttpClient(mockHttp),
            foodRepository,
            servingSizeRepository,
            searchedFoodRepository,
            unitOfWork,
            authDataStore
        );

        // Act
        var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFood.Should().HaveCount(1);
        capturedFood[0].Name.Should().Be(searchResponse.Items[0].Item.Description);
        capturedFood[0].ApiId.Should().Be(searchResponse.Items[0].Item.Id);
        capturedFood[0].FoodServingSizes.Should().HaveCount(1);
        capturedFood[0].FoodServingSizes.ToList()[0].Index.Should().Be(0);

        capturedServingSizes.Should().BeEmpty();
        await servingSizeRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>());

        capturedSearchedFood.Should().NotBeNull();
        capturedSearchedFood.Name.Should().Be(foodName.ToLower());

        await foodRepository
            .Received(1)
            .AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());
        await searchedFoodRepository
            .Received(1)
            .AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchFoodAndAddToDb_WhenFoodCreationFails_ShouldSkipFoodAndContinue()
    {
        // Arrange
        var foodName = "laptte";
        var authData = new AuthData()
        {
            TokenType = "token",
            AccessToken = "accessToken",
            ExpiresIn = 3600,
            RefreshToken = "refreshToken",
            UserId = "user",
        };

        var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;
        string returnedString = JsonConvert.SerializeObject(authData, jsonOptions);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(authUrl).Respond("application/json", returnedString);

        foodRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Food>());

        servingSizeRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<ServingSize>());

        var searchResponse = new FoodApiResponse()
        {
            Items =
            [
                new ItemListElement()
                {
                    Item = new ApiFood()
                    {
                        Id = 123124123213,
                        Description = "", // Invalid name to cause Food.Create to fail
                        Type = "food",
                        BrandName = "Test Brand",
                        CountryCode = "US",
                        NutritionalContents = new NutritionalContent
                        {
                            Energy = new Energy { Value = 100, Unit = "Kcal" },
                            Protein = 10,
                            Carbohydrates = 20,
                            Fat = 5,
                        },
                        ServingSizes =
                        [
                            new ApiServingSize()
                            {
                                Id = 123123123,
                                NutritionMultiplier = 1,
                                Unit = "ml",
                                Value = 100,
                            },
                        ],
                    },
                },
            ],
        };

        List<Food> capturedFood = [];
        List<ServingSize> capturedServingSizes = [];
        SearchedFood capturedSearchedFood = null!;

        foodRepository
            .When(x => x.AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedFood = callInfo.Arg<List<Food>>());

        servingSizeRepository
            .When(x => x.AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedServingSizes = callInfo.Arg<List<ServingSize>>());

        searchedFoodRepository
            .When(x => x.AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedSearchedFood = callInfo.Arg<SearchedFood>());

        mockHttp
            .When(HttpMethod.Get, "https://api.myfitnesspal.com/v2/nutrition*")
            .Respond("application/json", JsonConvert.SerializeObject(searchResponse, jsonOptions));

        var sut = new FoodApiService(
            foodApiOptions,
            new HttpClient(mockHttp),
            foodRepository,
            servingSizeRepository,
            searchedFoodRepository,
            unitOfWork,
            authDataStore
        );

        // Act
        var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFood.Should().BeEmpty();
        await foodRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());

        capturedServingSizes.Should().BeEmpty();
        await servingSizeRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>());

        capturedSearchedFood.Should().NotBeNull();
        capturedSearchedFood.Name.Should().Be(foodName.ToLower());

        await searchedFoodRepository
            .Received(1)
            .AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchFoodAndAddToDb_WhenServingSizeCreationFails_ShouldSkipWholeFood()
    {
        // Arrange
        var foodName = "laptte";
        var authData = new AuthData()
        {
            TokenType = "token",
            AccessToken = "accessToken",
            ExpiresIn = 3600,
            RefreshToken = "refreshToken",
            UserId = "user",
        };

        var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;
        string returnedString = JsonConvert.SerializeObject(authData, jsonOptions);

        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(authUrl).Respond("application/json", returnedString);

        foodRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Food>());

        servingSizeRepository
            .GetWhereApiIdPartOfListAsync(Arg.Any<List<long>>(), Arg.Any<CancellationToken>())
            .Returns(new List<ServingSize>());

        var searchResponse = new FoodApiResponse()
        {
            Items =
            [
                new ItemListElement()
                {
                    Item = new ApiFood()
                    {
                        Id = 123124123213,
                        Description = "lapte",
                        Type = "food",
                        BrandName = "Test Brand",
                        CountryCode = "US",
                        NutritionalContents = new NutritionalContent
                        {
                            Energy = new Energy { Value = 100, Unit = "Kcal" },
                            Protein = 10,
                            Carbohydrates = 20,
                            Fat = 5,
                        },
                        ServingSizes =
                        [
                            new ApiServingSize()
                            {
                                Id = 123123123,
                                NutritionMultiplier = -1, // Invalid multiplier to cause ServingSize.Create to fail
                                Unit = "ml",
                                Value = 100,
                            },
                        ],
                    },
                },
            ],
        };

        List<Food> capturedFood = [];
        List<ServingSize> capturedServingSizes = [];
        SearchedFood capturedSearchedFood = null!;

        foodRepository
            .When(x => x.AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedFood = callInfo.Arg<List<Food>>());

        servingSizeRepository
            .When(x => x.AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedServingSizes = callInfo.Arg<List<ServingSize>>());

        searchedFoodRepository
            .When(x => x.AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>()))
            .Do(callInfo => capturedSearchedFood = callInfo.Arg<SearchedFood>());

        mockHttp
            .When(HttpMethod.Get, "https://api.myfitnesspal.com/v2/nutrition*")
            .Respond("application/json", JsonConvert.SerializeObject(searchResponse, jsonOptions));

        var sut = new FoodApiService(
            foodApiOptions,
            new HttpClient(mockHttp),
            foodRepository,
            servingSizeRepository,
            searchedFoodRepository,
            unitOfWork,
            authDataStore
        );

        // Act
        var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capturedFood.Should().HaveCount(0);

        capturedServingSizes.Should().BeEmpty();
        await servingSizeRepository
            .DidNotReceive()
            .AddRangeAsync(Arg.Any<List<ServingSize>>(), Arg.Any<CancellationToken>());

        capturedSearchedFood.Should().NotBeNull();
        capturedSearchedFood.Name.Should().Be(foodName.ToLower());

        await foodRepository
            .Received(0)
            .AddRangeAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());
        await searchedFoodRepository
            .Received(1)
            .AddAsync(Arg.Any<SearchedFood>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
