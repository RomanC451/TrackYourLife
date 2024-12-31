using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetActiveGoalByType;

public sealed class GetActiveGoalByTypeQueryHandler(
    IGoalQuery userGoalQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetActiveGoalByTypeQuery, GoalReadModel>
{
    public async Task<Result<GoalReadModel>> Handle(
        GetActiveGoalByTypeQuery request,
        CancellationToken cancellationToken
    )
    {
        var goal = await userGoalQuery.GetActiveGoalByTypeAsync(
            userIdentifierProvider.UserId,
            request.Type,
            cancellationToken
        );

        if (goal is null)
        {
            return Result.Failure<GoalReadModel>(GoalErrors.NotExisting(request.Type));
        }

        return Result.Success(goal);
    }
}
