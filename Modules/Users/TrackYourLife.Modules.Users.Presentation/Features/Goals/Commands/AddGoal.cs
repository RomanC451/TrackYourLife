using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;

internal sealed record AddGoalRequest(
    int Value,
    GoalType Type,
    GoalPeriod Period,
    DateOnly StartDate,
    bool? Force,
    DateOnly? EndDate
);

internal sealed class AddGoal(ISender sender) : Endpoint<AddGoalRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<GoalsGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(AddGoalRequest req, CancellationToken ct)
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(req => new AddGoalCommand(
                req.Value,
                req.Type,
                req.Period,
                req.StartDate,
                req.EndDate,
                req.Force ?? false
            ))
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"/{ApiRoutes.Goals}/{id.Value}");
    }
}
