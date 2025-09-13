using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExercisesByUserId;
using TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Queries;

public sealed class GetExercises(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<ExercisesGroup>();
        Description(x => x.Produces<List<ExerciseDto>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetExercisesByUserIdQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(exercises =>
                exercises.Select(e => e.ToDto()).OrderBy(e => e.Name).ToList()
            );
    }
}
