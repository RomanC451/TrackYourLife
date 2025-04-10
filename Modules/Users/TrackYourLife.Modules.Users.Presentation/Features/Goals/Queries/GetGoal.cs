using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetGoalByType;
using TrackYourLife.Modules.Users.Contracts.Goals;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Queries;

internal sealed record GetGoalRequest
{
    [QueryParam]
    public GoalType GoalType { get; init; }

    [QueryParam]
    public DateOnly Date { get; init; }
};

internal sealed class GetGoal(ISender sender, IUsersMapper mapper)
    : Endpoint<GetGoalRequest, IResult>
{
    public override void Configure()
    {
        Get("");
        Group<GoalsGroup>();
        Description(x =>
            x.Produces<GoalDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(GetGoalRequest req, CancellationToken ct)
    {
        return await Result
            .Create(
                new GetGoalByTypeQuery(req.GoalType, req.Date),
                DomainErrors.General.UnProcessableRequest
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(goal => TypedResults.Ok(mapper.Map<GoalDto>(goal)));
    }
}
