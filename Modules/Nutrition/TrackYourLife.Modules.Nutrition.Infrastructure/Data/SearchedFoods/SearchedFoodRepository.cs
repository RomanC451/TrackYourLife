using TrackYourLife.Modules.Nutrition.Domain.Features.SearchedFoods;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.SearchedFoods.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.SearchedFoods;

internal sealed class SearchedFoodRepository(NutritionWriteDbContext context)
    : GenericRepository<SearchedFood, SearchedFoodId>(context.SearchedFoods),
        ISearchedFoodRepository
{
    public async Task<SearchedFood?> GetByNameAsync(
        string name,
        CancellationToken cancellationToken
    )
    {
        return await FirstOrDefaultAsync(
            new SearchedFoodWithNameSpecification(name),
            cancellationToken
        );
    }
}
