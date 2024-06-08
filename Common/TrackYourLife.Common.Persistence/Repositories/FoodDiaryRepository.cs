

using TrackYourLife.Common.Domain.FoodDiaries;
using TrackYourLife.Common.Persistence.Specifications;

namespace TrackYourLife.Common.Persistence.Repositories;

internal sealed class FoodDiaryRepository
    : GenericRepository<FoodDiaryEntry, FoodDiaryEntryId>,
        IFoodDiaryRepository
{
    public FoodDiaryRepository(ApplicationWriteDbContext context)
        : base(context.FoodDiaries) { }

    public async Task<List<FoodDiaryEntry>> GetByDateAsync(
        DateOnly date,
        CancellationToken cancellationToken
    )
    {
        return await WhereAsync(new FoodDiaryEntryWithDateSpecification(date), cancellationToken);
    }
}
