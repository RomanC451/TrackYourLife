namespace TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

public interface IFoodQuery
{
    Task<FoodReadModel?> GetByIdAsync(FoodId id, CancellationToken cancellationToken);

    Task<IEnumerable<FoodReadModel>> SearchFoodAsync(
        string searchTerm,
        CancellationToken cancellationToken
    );

    Task<List<FoodReadModel>> GetFoodsPartOfAsync(
        IEnumerable<FoodId> foodIds,
        CancellationToken cancellationToken
    );
}
