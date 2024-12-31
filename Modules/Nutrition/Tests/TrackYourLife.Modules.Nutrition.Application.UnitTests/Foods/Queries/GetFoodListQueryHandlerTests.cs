using FluentAssertions;
using NSubstitute;
using NSubstitute.ClearExtensions;
using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.Modules.Nutrition.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Nutrition.Application.UnitTests.Foods.Queries;

public class GetFoodListQueryHandlerTests : IDisposable
{
    private readonly IFoodApiService foodApiService = Substitute.For<IFoodApiService>();
    private readonly IFoodRepository foodRepository = Substitute.For<IFoodRepository>();
    private readonly ISearchedFoodRepository searchedFoodRepository =
        Substitute.For<ISearchedFoodRepository>();

    private readonly SearchFoodsByNameQueryHandler sut;

    public GetFoodListQueryHandlerTests()
    {
        sut = new SearchFoodsByNameQueryHandler(
            foodApiService,
            foodRepository,
            searchedFoodRepository
        );
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        foodApiService.ClearSubstitute();
        foodRepository.ClearSubstitute();
        searchedFoodRepository.ClearSubstitute();
    }

    [Fact]
    public async Task Handle_WhenSearchedFoodIsNotNull_ShouldReturnFoodList()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("test", 1, 10);

        var searchedFood = SearchedFood.Create(SearchedFoodId.NewId(), "test").Value;

        List<Food> foodList =
        [
            FoodFaker.Generate(name: "test"),
            FoodFaker.Generate(name: "test2"),
            FoodFaker.Generate(name: "test-asfasdasd"),
        ];

        searchedFoodRepository
            .GetByNameAsync("test", Arg.Any<CancellationToken>())
            .Returns(searchedFood);

        foodRepository
            .GetFoodListByContainingNameAsync("test", Arg.Any<CancellationToken>())
            .Returns(foodList);


        var expectedList = PagedList<Food>.Create(foodList, query.Page, query.PageSize).Value;

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedList);

        await searchedFoodRepository
            .Received(1)
            .GetByNameAsync("test", Arg.Any<CancellationToken>());
        await foodRepository
            .Received(1)
            .GetFoodListByContainingNameAsync("test", Arg.Any<CancellationToken>());
        await foodApiService
            .DidNotReceive()
            .SearchFoodAndAddToDbAsync("test", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSearchedFoodIsNullAndFoodApiSucceed_ShouldReturnFoodList()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("test", 1, 10);

        searchedFoodRepository
            .GetByNameAsync("test", Arg.Any<CancellationToken>())
            .Returns((SearchedFood)null!);

        List<Food> foodList =
        [
            FoodFaker.Generate(name: "test"),
            FoodFaker.Generate(name: "test2"),
            FoodFaker.Generate(name: "test-asfasdasd"),
        ];

        foodApiService
            .SearchFoodAndAddToDbAsync("test", Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        foodRepository
            .GetFoodListByContainingNameAsync(query.SearchParam, Arg.Any<CancellationToken>())
            .Returns(foodList);


        var expectedList = PagedList<Food>.Create(foodList, query.Page, query.PageSize).Value;

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedList);

        await searchedFoodRepository
            .Received(1)
            .GetByNameAsync("test", Arg.Any<CancellationToken>());
        await foodRepository
            .Received(1)
            .GetFoodListByContainingNameAsync("test", Arg.Any<CancellationToken>());
        await foodApiService
            .Received(1)
            .SearchFoodAndAddToDbAsync("test", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSearchedFoodIsNullAndFoodApiFails_ShouldReturnFailure()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("test", 1, 10);

        searchedFoodRepository
            .GetByNameAsync("test", Arg.Any<CancellationToken>())
            .Returns((SearchedFood)null!);

        foodApiService
            .SearchFoodAndAddToDbAsync("test", Arg.Any<CancellationToken>())
            .Returns(Result.Failure(InfrastructureErrors.FoodApiService.FoodNotFound));

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.FoodApiService.FoodNotFound);

        await searchedFoodRepository
            .Received(1)
            .GetByNameAsync("test", Arg.Any<CancellationToken>());
        await foodRepository
            .DidNotReceive()
            .GetFoodListByContainingNameAsync("test", Arg.Any<CancellationToken>());
        await foodApiService
            .Received(1)
            .SearchFoodAndAddToDbAsync("test", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSearchedFoodIsNullAndSearchStringIsEmpty_ShouldReturnFailure()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("", 1, 10);

        searchedFoodRepository
            .GetByNameAsync("", Arg.Any<CancellationToken>())
            .Returns((SearchedFood)null!);

        foodApiService
            .SearchFoodAndAddToDbAsync("", Arg.Any<CancellationToken>())
            .Returns(Result.Success());


        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Contain("Empty");
        result.Error.Code.Should().Contain("Name");

        await searchedFoodRepository.Received(1).GetByNameAsync("", Arg.Any<CancellationToken>());
        await foodRepository
            .DidNotReceive()
            .GetFoodListByContainingNameAsync("", Arg.Any<CancellationToken>());
        await foodApiService
            .Received(1)
            .SearchFoodAndAddToDbAsync("", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsEmptyList_ShouldReturnFailure()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("test", 1, 10);

        var searchedFood = SearchedFood.Create(SearchedFoodId.NewId(), "test").Value;

        List<Food> foodList = [];

        searchedFoodRepository
            .GetByNameAsync("test", Arg.Any<CancellationToken>())
            .Returns(searchedFood);

        foodRepository
            .GetFoodListByContainingNameAsync("test", Arg.Any<CancellationToken>())
            .Returns(foodList);

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(FoodErrors.NotFoundByName(query.SearchParam));

        await searchedFoodRepository
            .Received(1)
            .GetByNameAsync("test", Arg.Any<CancellationToken>());
        await foodRepository
            .Received(1)
            .GetFoodListByContainingNameAsync("test", Arg.Any<CancellationToken>());
        await foodApiService
            .DidNotReceive()
            .SearchFoodAndAddToDbAsync("test", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsFoodListAndPageIsOutOfIndex_ShouldReturnFailure()
    {
        // Arrange
        var query = new SearchFoodsByNameQuery("test", 2, 10);

        var searchedFood = SearchedFood.Create(SearchedFoodId.NewId(), "test").Value;

        List<Food> foodList =
        [
            FoodFaker.Generate(name: "test"),
            FoodFaker.Generate(name: "test2"),
            FoodFaker.Generate(name: "test-asfasdasd"),
        ];

        searchedFoodRepository
            .GetByNameAsync("test", Arg.Any<CancellationToken>())
            .Returns(searchedFood);

        foodRepository
            .GetFoodListByContainingNameAsync("test", Arg.Any<CancellationToken>())
            .Returns(foodList);


        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result
            .Error.Code.Should()
            .BeEquivalentTo(
                DomainErrors.ArgumentError.OutOfIndex(nameof(PagedList<Food>), "MaxPage")
            );

        await searchedFoodRepository
            .Received(1)
            .GetByNameAsync("test", Arg.Any<CancellationToken>());
        await foodRepository
            .Received(1)
            .GetFoodListByContainingNameAsync("test", Arg.Any<CancellationToken>());
        await foodApiService
            .DidNotReceive()
            .SearchFoodAndAddToDbAsync("test", Arg.Any<CancellationToken>());
    }
}
