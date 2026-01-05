using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Foods.Queries;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Presentation.UnitTests.Features.Foods.Queries;

public class SearchFoodsByNameTests
{
    private readonly ISender _sender;
    private readonly SearchFoodsByName _endpoint;

    public SearchFoodsByNameTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new SearchFoodsByName(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithPagedList()
    {
        // Arrange
        var foods = new List<FoodReadModel>
        {
            new(FoodId.NewId(), "Test Food 1", "Generic", "Test Brand", "US")
            {
                NutritionalContents = new(),
                FoodServingSizes = [],
            },
            new(FoodId.NewId(), "Test Food 2", "Generic", "Test Brand 2", "US")
            {
                NutritionalContents = new(),
                FoodServingSizes = [],
            },
        };

        var pagedListResult = PagedList<FoodReadModel>.Create(foods, 1, 10);
        var pagedList = pagedListResult.Value!;

        _sender
            .Send(Arg.Any<SearchFoodsByNameQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagedList));

        var request = new SearchFoodsByNameRequest
        {
            SearchParam = "test",
            Page = 1,
            PageSize = 10,
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<PagedList<FoodDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        okResult.Value!.Items.Should().HaveCount(2);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<SearchFoodsByNameQuery>(q =>
                    q.SearchParam == "test" && q.Page == 1 && q.PageSize == 10
                ),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnProblemDetails()
    {
        // Arrange
        var error = new Error("SearchError", "Search failed");
        _sender
            .Send(Arg.Any<SearchFoodsByNameQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<PagedList<FoodReadModel>>(error));

        var request = new SearchFoodsByNameRequest
        {
            SearchParam = "test",
            Page = 1,
            PageSize = 10,
        };

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
