using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.AddGoal;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Contracts.Shared;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;

internal sealed record AddGoalRequest(
    int Value,
    GoalType Type,
    GoalPeriod PerPeriod,
    DateOnly StartDate,
    bool? Force,
    DateOnly? EndDate
);

internal sealed class AddGoal(ISender sender, IUsersMapper mapper)
    : Endpoint<AddGoalRequest, IResult>
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
        var result = await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(mapper.Map<AddGoalCommand>)
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.Ok(new IdResponse(result.Value)),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails()),
        };
    }
}
