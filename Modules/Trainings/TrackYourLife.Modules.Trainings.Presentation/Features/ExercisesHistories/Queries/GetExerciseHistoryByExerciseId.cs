using TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Models;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.ExercisesHistories.Queries;

internal sealed record GetExerciseHistoryByExerciseIdRequest
{
    [QueryParam]
    public ExerciseId ExerciseId { get; init; } = ExerciseId.Empty;
}

internal sealed class GetExerciseHistoryByExerciseId(ISender sender)
    : Endpoint<GetExerciseHistoryByExerciseIdRequest, IResult>
{
    public override void Configure()
    {
        Get("");
        Group<ExercisesHistoriesGroup>();
        Description(x =>
            x.Produces<List<ExerciseHistoryDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetExerciseHistoryByExerciseIdRequest request,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetExerciseHistoryByExerciseIdQuery(request.ExerciseId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(exerciseHistories =>
                exerciseHistories.Select(e => e.ToDto()).ToList()
            );
    }
}
