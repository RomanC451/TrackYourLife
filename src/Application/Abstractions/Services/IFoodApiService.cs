using TrackYourLifeDotnet.Domain.Foods;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Application.Abstractions.Services;

public interface IFoodApiService
{
    Task<Result<List<Domain.Foods.Food>>> GetFoodListAsync(
        string foodName,
        int page,
        CancellationToken cancellationToken
    );
}
