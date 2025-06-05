using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;

internal sealed record UpdateGoalRequest(
    GoalId Id,
    GoalType Type,
    int Value,
    GoalPeriod Period,
    DateOnly StartDate,
    bool? Force,
    DateOnly? EndDate
);

internal sealed class UpdateGoal(ISender sender) : Endpoint<UpdateGoalRequest, IResult>
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
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(req => new UpdateGoalCommand(
                req.Id,
                req.Type,
                req.Value,
                req.Period,
                req.StartDate,
                req.EndDate,
                req.Force ?? false
            ))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
