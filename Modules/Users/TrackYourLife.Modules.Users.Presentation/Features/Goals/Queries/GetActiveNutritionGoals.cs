using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionActiveGoals;
using TrackYourLife.Modules.Users.Contracts.Goals;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Queries;

internal sealed class GetActiveNutritionGoals(ISender sender, IUsersMapper mapper)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("nutrition-active-goals");
        Group<GoalsGroup>();
        Description(x =>
            x.Produces<List<GoalDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetActiveNutritionGoalsQuery(), DomainErrors.General.UnProcessableRequest)
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(goalsList =>
                TypedResults.Ok(goalsList.Select(goal => mapper.Map<GoalDto>(goal)).ToList())
            );
    }
}
