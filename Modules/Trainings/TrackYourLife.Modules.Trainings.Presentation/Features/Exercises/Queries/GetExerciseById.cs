using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExerciseById;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Queries;

internal sealed record GetExerciseByIdRequest(ExerciseId Id);

internal sealed class GetExerciseById(ISender sender) : Endpoint<GetExerciseByIdRequest, IResult>
{
    public override void Configure()
    {
        Get("/{id}");
        Group<ExercisesGroup>();
        Description(x =>
            x.Produces<ExerciseDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        GetExerciseByIdRequest request,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new GetExerciseByIdQuery(request.Id))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(exercise => exercise.ToDto());
    }
}
