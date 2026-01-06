using NSubstitute;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Contracts.MappingsExtensions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Features.Foods.Queries.SearchFoodsByName;

public class SearchFoodsByNameQueryHandlerTests
{
    private readonly IFoodApiService _foodApiService;
    private readonly IFoodQuery _foodQuery;
    private readonly ISearchedFoodRepository _searchedFoodRepository;
    private readonly IFoodHistoryService _foodHistoryService;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly SearchFoodsByNameQueryHandler _handler;

    public SearchFoodsByNameQueryHandlerTests()
    {
        _foodApiService = Substitute.For<IFoodApiService>();
        _foodQuery = Substitute.For<IFoodQuery>();
        _searchedFoodRepository = Substitute.For<ISearchedFoodRepository>();
        _foodHistoryService = Substitute.For<IFoodHistoryService>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new SearchFoodsByNameQueryHandler(
            _foodApiService,
            _foodQuery,
            _searchedFoodRepository,
            _foodHistoryService,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WhenSearchParamIsEmpty_ShouldReturnFoodHistory()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery(string.Empty, 1, 10);
        var userId = UserId.NewId();
        var foodReadModels = new List<FoodReadModel>
        {
            FoodFaker.GenerateReadModel(),
            FoodFaker.GenerateReadModel(),
        };
        var expectedFoods = foodReadModels.Select(f => f.ToDto()).ToList();

        _userIdentifierProvider.UserId.Returns(userId);
        _foodHistoryService
            .GetFoodHistoryAsync(userId, Arg.Any<CancellationToken>())
            .Returns(expectedFoods);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEquivalentTo(expectedFoods);
    }

    [Fact]
    public async Task Handle_WhenSearchParamIsNotEmptyAndFoodExists_ShouldReturnFoods()
    {
        // Arrange
        var searchParam = "test";
        var query = new SearchFoodsByNameQuery(searchParam, 1, 10);
        var userId = UserId.NewId();
        var foodReadModels = new List<FoodReadModel>
        {
            FoodFaker.GenerateReadModel(),
            FoodFaker.GenerateReadModel(),
        };
        var expectedFoods = foodReadModels.Select(f => f.ToDto()).ToList();

        _userIdentifierProvider.UserId.Returns(userId);
        _searchedFoodRepository
            .GetByNameAsync(searchParam.ToLower(), Arg.Any<CancellationToken>())
            .Returns(SearchedFood.Create(SearchedFoodId.NewId(), searchParam.ToLower()).Value);
        _foodQuery
            .SearchFoodAsync(searchParam, Arg.Any<CancellationToken>())
            .Returns(foodReadModels);
        _foodHistoryService
            .PrioritizeHistoryItemsAsync(userId, foodReadModels, Arg.Any<CancellationToken>())
            .Returns(expectedFoods);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEquivalentTo(expectedFoods);
    }

    [Fact]
    public async Task Handle_WhenSearchParamIsNotEmptyAndFoodDoesNotExist_ShouldSearchApiAndReturnFoods()
    {
        // Arrange
        var searchParam = "test";
        var query = new SearchFoodsByNameQuery(searchParam, 1, 10);
        var userId = UserId.NewId();
        var foodReadModels = new List<FoodReadModel>
        {
            FoodFaker.GenerateReadModel(),
            FoodFaker.GenerateReadModel(),
        };
        var expectedFoods = foodReadModels.Select(f => f.ToDto()).ToList();

        _userIdentifierProvider.UserId.Returns(userId);
        _searchedFoodRepository
            .GetByNameAsync(searchParam.ToLower(), Arg.Any<CancellationToken>())
            .Returns((SearchedFood?)null);
        _foodApiService
            .SearchFoodAndAddToDbAsync(searchParam, Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _foodQuery
            .SearchFoodAsync(searchParam, Arg.Any<CancellationToken>())
            .Returns(foodReadModels);
        _foodHistoryService
            .PrioritizeHistoryItemsAsync(userId, foodReadModels, Arg.Any<CancellationToken>())
            .Returns(expectedFoods);

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEquivalentTo(expectedFoods);
    }

    [Fact]
    public async Task Handle_WhenApiSearchFails_ShouldReturnFailure()
    {
        // Arrange
        var searchParam = "test";
        var query = new SearchFoodsByNameQuery(searchParam, 1, 10);
        var userId = UserId.NewId();
        var expectedError = new Error("Test", "Test error");

        _userIdentifierProvider.UserId.Returns(userId);
        _searchedFoodRepository
            .GetByNameAsync(searchParam.ToLower(), Arg.Any<CancellationToken>())
            .Returns((SearchedFood?)null);
        _foodApiService
            .SearchFoodAndAddToDbAsync(searchParam, Arg.Any<CancellationToken>())
            .Returns(Result.Failure(expectedError));

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(expectedError);
    }

    [Fact]
    public async Task Handle_WhenSearchReturnsEmpty_ShouldReturnNotFoundError()
    {
        // Arrange
        var searchParam = "nonexistent";
        var query = new SearchFoodsByNameQuery(searchParam, 1, 10);
        var userId = UserId.NewId();

        _userIdentifierProvider.UserId.Returns(userId);
        _searchedFoodRepository
            .GetByNameAsync(searchParam.ToLower(), Arg.Any<CancellationToken>())
            .Returns(SearchedFood.Create(SearchedFoodId.NewId(), searchParam.ToLower()).Value);
        _foodQuery
            .SearchFoodAsync(searchParam, Arg.Any<CancellationToken>())
            .Returns(new List<FoodReadModel>());
        _foodHistoryService
            .PrioritizeHistoryItemsAsync(
                userId,
                Arg.Any<IEnumerable<FoodReadModel>>(),
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<FoodDto>());

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FoodErrors.NotFoundByName(searchParam));
    }
}
