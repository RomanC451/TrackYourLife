using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

public sealed class GetOngoingTrainingByUserIdQueryHandler(
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    IExercisesHistoriesQuery exercisesHistoriesQuery,
    IUserIdentifierProvider userIdentifierProvider,
    ISupaBaseStorage supaBaseStorage
)
    : IQueryHandler<
        GetOngoingTrainingByUserIdQuery,
        (
            OngoingTrainingReadModel OngoingTraining,
            IEnumerable<ExerciseHistoryReadModel> ExerciseHistories
        )
    >
{
    public async Task<
        Result<(
            OngoingTrainingReadModel OngoingTraining,
            IEnumerable<ExerciseHistoryReadModel> ExerciseHistories
        )>
    > Handle(GetOngoingTrainingByUserIdQuery request, CancellationToken cancellationToken)
    {
        var ongoingTraining = await ongoingTrainingsQuery.GetUnfinishedByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure<(
                OngoingTrainingReadModel,
                IEnumerable<ExerciseHistoryReadModel>
            )>(OngoingTrainingErrors.NotFound);
        }

        if (ongoingTraining.Training?.TrainingExercises is not null)
        {
            var signedUrlTasks = ongoingTraining
                .Training.TrainingExercises.Where(exercise =>
                    exercise?.Exercise?.PictureUrl is not null
                )
                .Select(async exercise =>
                {
                    var result = await supaBaseStorage.CreateSignedUrlAsync(
                        "images",
                        exercise!.Exercise!.PictureUrl!
                    );

                    if (result.IsSuccess)
                    {
                        exercise.Exercise = exercise.Exercise with { PictureUrl = result.Value };
                    }
                })
                .ToList();

            await Task.WhenAll(signedUrlTasks);
        }

        var exerciseHistories = await exercisesHistoriesQuery.GetByOngoingTrainingIdAsync(
            ongoingTraining.Id,
            cancellationToken
        );

        return Result.Success((ongoingTraining, exerciseHistories));
    }
}
