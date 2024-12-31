// TODO: FIX IT

// using FluentAssertions;
// using Microsoft.Extensions.Options;
// using Newtonsoft.Json;
// using Newtonsoft.Json.Serialization;
// using NSubstitute;
// using NSubstitute.ClearExtensions;
// using RichardSzalay.MockHttp;
// using TrackYourLife.Modules.Nutrition.Domain.Core;
// using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
// using TrackYourLife.Modules.Nutrition.Infrastructure.Options;
// using TrackYourLife.Modules.Nutrition.Infrastructure.Services;
// using TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;
// using TrackYourLife.SharedLib.Domain.Errors;

// namespace TrackYourLife.Modules.Nutrition.Infrastructure.UnitTests.Services;

// public class FoodApiServiceTests : IDisposable
// {
//     private readonly IOptions<FoodApiOptions> foodApiOptions = Substitute.For<
//         IOptions<FoodApiOptions>
//     >();

//     private readonly IFoodRepository foodRepository = Substitute.For<IFoodRepository>();
//     private readonly INutritionUnitOfWork unitOfWork = Substitute.For<INutritionUnitOfWork>();


//     private readonly FoodApiOptions options = new FoodApiOptions()
//     {
//         BaseUrl = "https://api.myfitnesspal.com",
//         BaseApiUrl = "https://api.myfitnesspal.com",
//         AuthTokenPath = "/user/auth_token?refresh=true",
//         SearchPath = "/v2/nutrition",
//         SpaceEncoded = "%20"
//     };

//     public FoodApiServiceTests()
//     {
//         foodApiOptions.Value.Returns(options);
//     }

//     public void Dispose()
//     {
//         Dispose(true);
//         GC.SuppressFinalize(this);
//     }

//     protected virtual void Dispose(bool disposing)
//     {
//         foodRepository.ClearSubstitute();
//         unitOfWork.ClearSubstitute();
//     }

//     public class SnakeCaseNamingStrategy : NamingStrategy
//     {
//         protected override string ResolvePropertyName(string name)
//         {
//             return string.Concat(
//                     name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())
//                 )
//                 .ToLower();
//         }
//     }

//     private readonly JsonSerializerSettings jsonOptions = new JsonSerializerSettings
//     {
//         ContractResolver = new DefaultContractResolver
//         {
//             NamingStrategy = new SnakeCaseNamingStrategy()
//         },
//         Formatting = Formatting.Indented
//     };

//     [Fact]
//     public async Task SearchFoodAndAddToDb_WhenFailsToAuthenticate_ReturnsFailure()
//     {
//         // Arrange

//         var foodName = "laptte";

//         var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;

//         var mockHttp = new MockHttpMessageHandler();

//         mockHttp.When(authUrl).Respond("application/json", ""); // Respond with JSON

//         var sut = new FoodApiService(
//             foodApiOptions,
//             new HttpClient(mockHttp),
//             foodRepository,
//             unitOfWork
//         );

//         // Act
//         var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

//         // Assert
//         result.IsFailure.Should().BeTrue();
//         result
//             .Error.Should()
//             .BeEquivalentTo(InfrastructureErrors.FoodApiService.FoodApiNotAuthenticated);
//         await foodRepository.DidNotReceive().AddFoodListAsync(default!, default);
//     }

//     [Fact]
//     public async Task SearchFoodAndAddToDb_WhenFailsToDeserializeTheResponse_ReturnsFailure()
//     {
//         // Arrange

//         var foodName = "laptte";
//         var authData = new AuthData()
//         {
//             TokenType = "token",
//             AccessToken = "accessToken",
//             ExpiresIn = 3600,
//             RefreshToken = "refreshToken",
//             UserId = "user",
//         };

//         var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;

//         string reuturnedString = JsonConvert.SerializeObject(authData, jsonOptions);

//         var mockHttp = new MockHttpMessageHandler();

//         mockHttp.When(authUrl).Respond("application/json", reuturnedString);

//         var searchUrl =
//             $"{foodApiOptions.Value.BaseApiUrl}{foodApiOptions.Value.SearchPath}?q={foodName.Replace(" ", foodApiOptions.Value.SpaceEncoded)}";

//         mockHttp.When(searchUrl).Respond("application/json", "asd");

//         var sut = new FoodApiService(
//             foodApiOptions,
//             new HttpClient(mockHttp),
//             foodRepository,
//             unitOfWork
//         );

//         // Act


//         var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

//         // Assert

//         result.IsFailure.Should().BeTrue();
//         result
//             .Error.Should()
//             .BeEquivalentTo(InfrastructureErrors.FoodApiService.FailedJsonDeserialization);
//         await foodRepository.DidNotReceive().AddFoodListAsync(default!, default);
//     }

//     [Fact]
//     public async Task SearchFoodAndAddToDb_WhenResponseItemsListIsEmpty_ReturnsFailure()
//     {
//         // Arrange

//         var foodName = "laptte";
//         var authData = new AuthData()
//         {
//             TokenType = "token",
//             AccessToken = "accessToken",
//             ExpiresIn = 3600,
//             RefreshToken = "refreshToken",
//             UserId = "user",
//         };

//         var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;

//         string reuturnedString = JsonConvert.SerializeObject(authData, jsonOptions);

//         var mockHttp = new MockHttpMessageHandler();

//         mockHttp.When(authUrl).Respond("application/json", reuturnedString);

//         var searchUrl =
//             $"{foodApiOptions.Value.BaseApiUrl}{foodApiOptions.Value.SearchPath}?q={foodName.Replace(" ", foodApiOptions.Value.SpaceEncoded)}";

//         var searchResponse = new FoodApiResponse();

//         mockHttp
//             .When(searchUrl)
//             .Respond("application/json", JsonConvert.SerializeObject(searchResponse, jsonOptions));

//         var sut = new FoodApiService(
//             foodApiOptions,
//             new HttpClient(mockHttp),
//             foodRepository,
//             unitOfWork
//         );

//         // Act


//         var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

//         // Assert

//         result.IsFailure.Should().BeTrue();
//         result.Error.Should().BeEquivalentTo(InfrastructureErrors.FoodApiService.FoodNotFound);

//         await foodRepository.DidNotReceive().AddFoodListAsync(default!, default);
//     }

//     [Fact]
//     public async Task SearchFoodAndAddToDb_WhenResponseIsValid_ConvertsToFoodsAddsToDbAndReturnSuccess()
//     {
//         // Arrange

//         var foodName = "laptte";
//         var authData = new AuthData()
//         {
//             TokenType = "token",
//             AccessToken = "accessToken",
//             ExpiresIn = 3600,
//             RefreshToken = "refreshToken",
//             UserId = "user",
//         };

//         var authUrl = foodApiOptions.Value.BaseUrl + foodApiOptions.Value.AuthTokenPath;

//         string reuturnedString = JsonConvert.SerializeObject(authData, jsonOptions);

//         var mockHttp = new MockHttpMessageHandler();

//         mockHttp.When(authUrl).Respond("application/json", reuturnedString);

//         var searchUrl =
//             $"{foodApiOptions.Value.BaseApiUrl}{foodApiOptions.Value.SearchPath}?q={foodName.Replace(" ", foodApiOptions.Value.SpaceEncoded)}";

//         var searchResponse = new FoodApiResponse()
//         {
//             Items =
//             [
//                 new ItemListElement()
//                 {
//                     Item = new ApiFood()
//                     {
//                         Id = 123124123213,
//                         Description = "lapte",
//                         Type = "food",
//                         ServingSizes =
//                         [
//                             new ApiServingSize()
//                             {
//                                 Id = 123123123,
//                                 NutritionMultiplier = 1,
//                                 Unit = "ml",
//                                 Value = 100
//                             }
//                         ]
//                     }
//                 }
//             ]
//         };

//         List<Food> capturedFood = null!;

//         foodRepository
//             .When(x => x.AddFoodListAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>()))
//             .Do(callInfo => capturedFood = callInfo.Arg<List<Food>>());

//         mockHttp
//             .When(searchUrl)
//             .Respond("application/json", JsonConvert.SerializeObject(searchResponse, jsonOptions));

//         var sut = new FoodApiService(
//             foodApiOptions,
//             new HttpClient(mockHttp),
//             foodRepository,
//             unitOfWork
//         );

//         // Act


//         var result = await sut.SearchFoodAndAddToDbAsync(foodName, CancellationToken.None);

//         // Assert

//         result.IsSuccess.Should().BeTrue();
//         capturedFood.Should().HaveCount(1);
//         capturedFood[0].Name.Should().Be(searchResponse.Items[0].Item.Description);
//         capturedFood[0].ApiId.Should().Be(searchResponse.Items[0].Item.Id);
//         capturedFood[0].FoodServingSizes.Should().HaveCount(1);
//         capturedFood[0].FoodServingSizes.ToList()[0].Index.Should().Be(0);
//         capturedFood[0]
//             .FoodServingSizes.ToList()[0]
//             .ServingSize.NutritionMultiplier.Should()
//             .Be(searchResponse.Items[0].Item.ServingSizes[0].NutritionMultiplier);
//         capturedFood[0]
//             .FoodServingSizes.ToList()[0]
//             .ServingSize.Unit.Should()
//             .Be(searchResponse.Items[0].Item.ServingSizes[0].Unit);
//         capturedFood[0]
//             .FoodServingSizes.ToList()[0]
//             .ServingSize.Value.Should()
//             .Be(searchResponse.Items[0].Item.ServingSizes[0].Value);

//         await foodRepository
//             .Received(1)
//             .AddFoodListAsync(Arg.Any<List<Food>>(), Arg.Any<CancellationToken>());
//     }
// }
