using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services
{
    public interface IGoalsManagerService
    {
        Task<Result<bool>> HandleOverlappingGoalsAsync(
            Goal newUserGoal,
            bool force,
            CancellationToken cancellationToken
        );
    }
}
