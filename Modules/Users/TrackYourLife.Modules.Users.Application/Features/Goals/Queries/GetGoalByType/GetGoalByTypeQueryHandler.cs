using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetGoalByType;

internal sealed class GetGoalByTypeQueryHandler(
    IGoalQuery userGoalQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetGoalByTypeQuery, GoalReadModel>
{
    public async Task<Result<GoalReadModel>> Handle(
        GetGoalByTypeQuery request,
        CancellationToken cancellationToken
    )
    {
        var goal = await userGoalQuery.GetGoalByTypeAndDateAsync(
            userIdentifierProvider.UserId,
            request.Type,
            request.Date,
            cancellationToken
        );

        if (goal is null)
        {
            return Result.Failure<GoalReadModel>(GoalErrors.NotExisting(request.Type));
        }

        return Result.Success(goal);
    }
}
