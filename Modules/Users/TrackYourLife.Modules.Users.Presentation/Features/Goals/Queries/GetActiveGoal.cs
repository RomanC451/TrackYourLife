using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetActiveGoalByType;
using TrackYourLife.Modules.Users.Contracts.Goals;
using TrackYourLife.Modules.Users.Domain.Goals;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Queries;

internal sealed record GetActiveGoalRequest
{
    [QueryParam]
    public GoalType GoalType { get; init; }
};

internal sealed class GetActiveGoal(ISender sender, IUsersMapper mapper)
    : Endpoint<GetActiveGoalRequest, IResult>
{
    public override void Configure()
    {
        Get("active-goal");
        Group<GoalsGroup>();
        Description(x =>
            x.Produces<GoalDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(GetActiveGoalRequest req, CancellationToken ct)
    {
        return await Result
            .Create(
                new GetActiveGoalByTypeQuery(req.GoalType),
                DomainErrors.General.UnProcessableRequest
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(goal => TypedResults.Ok(mapper.Map<GoalDto>(goal)));
    }
}
