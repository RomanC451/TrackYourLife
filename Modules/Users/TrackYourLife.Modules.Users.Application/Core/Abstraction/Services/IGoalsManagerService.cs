using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services
{
    public interface IGoalsManagerService
    {
        Task<Result> HandleOverlappingGoalsAsync(
            Goal newUserGoal,
            bool force,
            CancellationToken cancellationToken
        );
    }
}
