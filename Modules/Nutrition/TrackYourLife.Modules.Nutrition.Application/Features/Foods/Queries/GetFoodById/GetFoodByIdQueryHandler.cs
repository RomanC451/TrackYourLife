using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.GetFoodById;

/// <summary>
/// Represents a query handler for retrieving a food item by its ID.
/// </summary>
/// <param name="foodQuery">The query service for querying the data base.</param>
public sealed class GetFoodByIdQueryHandler(IFoodQuery foodQuery)
    : IQueryHandler<GetFoodByIdQuery, FoodReadModel>
{
    public async Task<Result<FoodReadModel>> Handle(
        GetFoodByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var food = await foodQuery.GetByIdAsync(query.FoodId, cancellationToken);

        if (food is null)
        {
            return Result.Failure<FoodReadModel>(FoodErrors.NotFoundById(query.FoodId));
        }

        return Result.Success(food);
    }
}
