using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;

public interface IFoodApiService
{
    Task<Result> SearchFoodAndAddToDbAsync(string foodName, CancellationToken cancellationToken);
}
