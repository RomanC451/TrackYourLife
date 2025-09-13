using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExerciseById;

internal sealed class GetExerciseByIdQueryHandler(
    IExercisesQuery exerciseQuery,
    ISupaBaseStorage supaBaseStorage
) : IQueryHandler<GetExerciseByIdQuery, ExerciseReadModel>
{
    public async Task<Result<ExerciseReadModel>> Handle(
        GetExerciseByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await exerciseQuery.GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
        {
            return Result.Failure<ExerciseReadModel>((ExercisesErrors.NotFoundById(request.Id)));
        }

        if (exercise.PictureUrl is not null)
        {
            var result = await supaBaseStorage.CreateSignedUrlAsync("images", exercise.PictureUrl);

            if (result.IsSuccess)
            {
                exercise = exercise with { PictureUrl = result.Value };
            }
            else
            {
                exercise = exercise with { PictureUrl = "" };
            }
        }
        return Result.Success(exercise);
    }
}
