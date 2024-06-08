using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Application.Core.Abstractions.Services;

public interface IFoodApiService
{
    Task<Result<List<Domain.Foods.Food>>> GetFoodListAsync(
        string foodName,
        int page,
        CancellationToken cancellationToken
    );
}
