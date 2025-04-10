using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;

/// <summary>
/// Represents a query handler for search a food name in Db and retrieve a list with best matches.
/// </summary>
/// <param name="query">The query object containing search parameters.</param>
/// <param name="cancellationToken">The cancellation token.</param>
internal sealed class SearchFoodsByNameQueryHandler(
    IFoodApiService foodApiService,
    IFoodQuery foodQuery,
    ISearchedFoodRepository searchedFoodRepository,
    IFoodHistoryService foodHistoryService,
    IUserIdentifierProvider userIdentifierProvider
// IMemoryCache memoryCache
) : IQueryHandler<SearchFoodsByNameQuery, PagedList<FoodReadModel>>
{
    public async Task<Result<PagedList<FoodReadModel>>> Handle(
        SearchFoodsByNameQuery query,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<FoodReadModel> foodList;

        if (string.IsNullOrWhiteSpace(query.SearchParam))
        {
            foodList = await foodHistoryService.GetFoodHistoryAsync(
                userIdentifierProvider.UserId,
                cancellationToken
            );
        }
        else
        {
            var searchedFood = await searchedFoodRepository.GetByNameAsync(
                query.SearchParam.ToLower(),
                cancellationToken
            );

            if (searchedFood is null)
            {
                Result apiResult = await foodApiService.SearchFoodAndAddToDbAsync(
                    query.SearchParam,
                    cancellationToken
                );

                if (apiResult.IsFailure)
                {
                    return Result.Failure<PagedList<FoodReadModel>>(apiResult.Error);
                }
            }

            var foodListResult = await SearchFood(query.SearchParam, cancellationToken);

            if (foodListResult.IsFailure)
            {
                return Result.Failure<PagedList<FoodReadModel>>(foodListResult.Error);
            }

            foodList = foodListResult.Value;
        }

        var pagedListResult = PagedList<FoodReadModel>.Create(foodList, query.Page, query.PageSize);

        if (pagedListResult.IsFailure)
        {
            return Result.Failure<PagedList<FoodReadModel>>(pagedListResult.Error);
        }

        return Result.Success(pagedListResult.Value);
    }

    private async Task<Result<IEnumerable<FoodReadModel>>> SearchFood(
        string searchTerm,
        CancellationToken cancellationToken
    )
    {
        // !!TODO: Implement caching
        // string key = $"SearchFood_{userIdentifierProvider.UserId}_{searchTerm}";

        // var list = await memoryCache.GetOrCreateAsync(
        //     key,
        //     async entry =>
        //     {
        //         entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

        var foodList = await foodQuery.SearchFoodAsync(searchTerm, cancellationToken);

        var list = await foodHistoryService.PrioritizeHistoryItemsAsync(
            userIdentifierProvider.UserId,
            foodList,
            cancellationToken
        );
        //     }
        // );

        if (list is null || !list.Any())
        {
            return Result.Failure<IEnumerable<FoodReadModel>>(
                FoodErrors.NotFoundByName(searchTerm)
            );
        }

        return Result.Success(list);
    }
}
