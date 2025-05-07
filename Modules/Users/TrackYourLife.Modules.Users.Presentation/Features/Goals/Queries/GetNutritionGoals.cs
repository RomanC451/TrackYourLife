using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionGoals;
using TrackYourLife.Modules.Users.Contracts.Dtos;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Queries;

internal sealed record GetNutritionGoalsRequest
{
    [QueryParam]
    public DateOnly Date { get; init; }
}

internal sealed class GetNutritionGoals(ISender sender)
    : Endpoint<GetNutritionGoalsRequest, IResult>
{
    public override void Configure()
    {
        Get("nutrition-goals");
        Group<GoalsGroup>();
        Description(x =>
            x.Produces<List<GoalDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetNutritionGoalsRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetNutritionGoalsQuery(req.Date), DomainErrors.General.UnProcessableRequest)
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(goalsList => goalsList.Select(goal => goal.ToDto()).ToList());
    }
}
