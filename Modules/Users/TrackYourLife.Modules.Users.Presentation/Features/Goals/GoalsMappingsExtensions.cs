using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Goals;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals;

internal static class GoalsMappingsExtensions
{
    public static GoalDto ToDto(this GoalReadModel goal)
    {
        return new GoalDto(
            goal.Id,
            goal.Type,
            goal.Value,
            goal.Period,
            goal.StartDate,
            goal.EndDate
        );
    }
}
