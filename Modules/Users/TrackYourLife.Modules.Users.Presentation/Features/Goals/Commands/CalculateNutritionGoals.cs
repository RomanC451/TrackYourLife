using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;
using TrackYourLife.Modules.Users.Domain.Features.Goals.Enums;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;

internal sealed record CalculateNutritionGoalsRequest(
    int Age,
    float Weight,
    float Height,
    Gender Gender,
    ActivityLevel ActivityLevel,
    FitnessGoal FitnessGoal,
    bool Force
);

internal sealed class CalculateNutritionGoals(ISender sender)
    : Endpoint<CalculateNutritionGoalsRequest, IResult>
{
    public override void Configure()
    {
        Put("nutrition");
        Group<GoalsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        CalculateNutritionGoalsRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(req => new CalculateNutritionGoalsCommand(
                req.Age,
                req.Weight,
                req.Height,
                req.Gender,
                req.ActivityLevel,
                req.FitnessGoal,
                req.Force
            ))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
