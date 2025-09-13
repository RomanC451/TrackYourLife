using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

public sealed class GetOngoingTrainingByUserIdQueryHandler(
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    IUserIdentifierProvider userIdentifierProvider,
    ISupaBaseStorage supaBaseStorage
) : IQueryHandler<GetOngoingTrainingByUserIdQuery, OngoingTrainingReadModel>
{
    public async Task<Result<OngoingTrainingReadModel>> Handle(
        GetOngoingTrainingByUserIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingsQuery.GetUnfinishedByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure<OngoingTrainingReadModel>(OngoingTrainingErrors.NotFound);
        }

#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (var exercise in ongoingTraining.Training.TrainingExercises)
        {
            if (exercise.Exercise.PictureUrl is not null)
            {
                var result = await supaBaseStorage.CreateSignedUrlAsync(
                    "images",
                    exercise.Exercise.PictureUrl
                );

                if (result.IsSuccess)
                {
                    exercise.Exercise = exercise.Exercise with { PictureUrl = result.Value };
                }
            }
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions

        return Result.Success(ongoingTraining);
    }
}
