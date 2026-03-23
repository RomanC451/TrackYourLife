using TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetWorkoutStreak;
using TrackYourLife.Modules.Trainings.Contracts.Dtos;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Trainings.Queries;

internal sealed class GetWorkoutStreak(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("streak");
        Group<TrainingsGroup>();
        Description(x =>
            x.Produces<WorkoutStreakDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetWorkoutStreakQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(streak => streak);
    }
}
