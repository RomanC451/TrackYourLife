using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Contracts.MappingsExtensions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;

/// <summary>
/// Represents a query handler for retrieving a food item by its ID.
/// </summary>
/// <param name="foodQuery">The query service for querying the data base.</param>
/// <param name="foodHistoryQuery">The query service for querying food history.</param>
/// <param name="userIdentifierProvider">The provider for identifying the current user.</param>
internal sealed class GetFoodByIdQueryHandler(
    IFoodQuery foodQuery,
    IFoodHistoryQuery foodHistoryQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetFoodByIdQuery, FoodDto>
{
    public async Task<Result<FoodDto>> Handle(
        GetFoodByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var food = await foodQuery.GetByIdAsync(query.FoodId, cancellationToken);

        if (food is null)
        {
            return Result.Failure<FoodDto>(FoodErrors.NotFoundById(query.FoodId));
        }

        var dto = food.ToDto();

        var history = await foodHistoryQuery.GetByUserAndFoodAsync(
            userIdentifierProvider.UserId,
            query.FoodId,
            cancellationToken
        );

        if (history is not null)
        {
            return Result.Success(
                dto with
                {
                    LastServingSizeUsedId = history.LastServingSizeUsedId,
                    LastQuantityUsed = history.LastQuantityUsed,
                }
            );
        }

        return Result.Success(dto);
    }
}
