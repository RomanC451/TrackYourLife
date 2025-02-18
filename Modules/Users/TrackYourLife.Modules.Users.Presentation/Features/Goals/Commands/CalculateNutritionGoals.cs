using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.CalculateNutritionGoals;
using TrackYourLife.Modules.Users.Domain.Goals.Enums;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;

internal sealed record CalculateNutritionGoalsRequest(
    int Age,
    int Weight,
    int Height,
    Gender Gender,
    ActivityLevel ActivityLevel,
    FitnessGoal FitnessGoal,
    bool Force
);

internal sealed class CalculateNutritionGoals(ISender sender, IUsersMapper mapper)
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
        var result = await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(mapper.Map<CalculateNutritionGoalsCommand>)
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails())
        };
    }
}
