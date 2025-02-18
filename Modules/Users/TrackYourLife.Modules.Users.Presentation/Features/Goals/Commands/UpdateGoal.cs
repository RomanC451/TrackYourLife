using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;
using TrackYourLife.Modules.Users.Domain.Goals;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;

internal sealed record UpdateGoalRequest(
    GoalId Id,
    GoalType Type,
    int Value,
    GoalPeriod PerPeriod,
    DateOnly StartDate,
    bool? Force,
    DateOnly? EndDate
);

internal sealed class UpdateGoal(ISender sender, IUsersMapper mapper)
    : Endpoint<UpdateGoalRequest, IResult>
{
    public override void Configure()
    {
        Put("");
        Group<GoalsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(UpdateGoalRequest req, CancellationToken ct)
    {
        var result = await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(mapper.Map<UpdateGoalCommand>)
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails())
        };
    }
}
