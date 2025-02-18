using TrackYourLife.Modules.Users.Application.Core.Abstraction;
using TrackYourLife.Modules.Users.Application.Features.Goals.Commands.UpdateNutritionGoals;

namespace TrackYourLife.Modules.Users.Presentation.Features.Goals.Commands;

internal sealed record UpdateNutritionGoalsRequest(
    int Calories,
    int Protein,
    int Carbohydrates,
    int Fats,
    bool Force
);

internal sealed class UpdateNutritionGoals(ISender sender, IUsersMapper mapper)
    : Endpoint<UpdateNutritionGoalsRequest, IResult>
{
    public override void Configure()
    {
        Put("nutrition/update");
        Group<GoalsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateNutritionGoalsRequest req,
        CancellationToken ct
    )
    {
        var result = await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(mapper.Map<UpdateNutritionGoalsCommand>)
            .BindAsync(command => sender.Send(command, ct));

        return result switch
        {
            { IsSuccess: true } => TypedResults.NoContent(),
            { Error.HttpStatus: 404 } => TypedResults.NotFound(result.ToNoFoundProblemDetails()),
            _ => TypedResults.BadRequest(result.ToBadRequestProblemDetails())
        };
    }
}
