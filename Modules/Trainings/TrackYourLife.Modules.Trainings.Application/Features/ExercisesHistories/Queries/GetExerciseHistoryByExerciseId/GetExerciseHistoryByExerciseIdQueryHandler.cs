using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;

public sealed class GetExerciseHistoryByExerciseIdQueryHandler(
    IExercisesQuery exercisesQuery,
    IExercisesHistoriesQuery exercisesHistoriesQuery
) : IQueryHandler<GetExerciseHistoryByExerciseIdQuery, IEnumerable<ExerciseHistoryReadModel>>
{
    public async Task<Result<IEnumerable<ExerciseHistoryReadModel>>> Handle(
        GetExerciseHistoryByExerciseIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await exercisesQuery.GetByIdAsync(request.ExerciseId, cancellationToken);

        if (exercise is null)
        {
            return Result.Failure<IEnumerable<ExerciseHistoryReadModel>>(
                ExercisesErrors.NotFoundById(request.ExerciseId)
            );
        }

        var exerciseHistories = await exercisesHistoriesQuery.GetByExerciseIdAsync(
            request.ExerciseId,
            cancellationToken
        );

        return Result.Success(exerciseHistories);
    }
}
