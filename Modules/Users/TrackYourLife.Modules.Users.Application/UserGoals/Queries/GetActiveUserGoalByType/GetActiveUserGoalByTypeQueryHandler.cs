using TrackYourLife.Common.Application.Core.Abstractions.Authentication;
using TrackYourLife.Common.Application.Core.Abstractions.Messaging;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Shared;
using TrackYourLife.Modules.Users.Contracts.UserGoals;
using TrackYourLife.Modules.Users.Domain.UserGoals;


namespace TrackYourLife.Modules.Users.Application.UserGoals.Queries.GetActiveUserGoalByType;

public sealed class GetActiveUserGoalByTypeQueryHandler(
    IUserGoalQuery userGoalQuery,
    IUserIdentifierProvider userIdentifierProvider
    )
        : IQueryHandler<GetActiveUserGoalByTypeQuery, UserGoalResponse>
{
    private readonly IUserGoalQuery _userGoalQuery = userGoalQuery;

    private readonly IUserIdentifierProvider _userIdentifierProvider = userIdentifierProvider;

    public async Task<Result<UserGoalResponse>> Handle(
        GetActiveUserGoalByTypeQuery request,
        CancellationToken cancellationToken
    )
    {
        var userGoal = await _userGoalQuery.GetActiveGoalByTypeAsync(
            _userIdentifierProvider.UserId,
            request.Type,
            cancellationToken
        );

        if (userGoal is null)
        {
            return Result.Failure<UserGoalResponse>(
                DomainErrors.UserGoal.NotExisting(request.Type)
            );
        }

        return Result.Success(
            new UserGoalResponse(
                userGoal.Id,
                userGoal.Type,
                userGoal.Value,
                userGoal.PerPeriod,
                userGoal.StartDate,
                userGoal.EndDate
            )
        );
    }
}
