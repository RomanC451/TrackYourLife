

using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Persistence.Specifications;

namespace TrackYourLife.Common.Persistence.Repositories;

internal sealed class SearchedFoodRepository
    : GenericRepository<SearchedFood, SearchedFoodId>,
        ISearchedFoodRepository
{
    public SearchedFoodRepository(ApplicationWriteDbContext context)
        : base(context.SearchedFood) { }

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
