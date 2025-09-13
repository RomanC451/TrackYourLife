using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;

public sealed class GetOngoingTrainingByIdQueryHandler(
    IOngoingTrainingsQuery ongoingTrainingsQuery,
    ISupaBaseStorage supaBaseStorage
) : IQueryHandler<GetOngoingTrainingByIdQuery, OngoingTrainingReadModel>
{
    public async Task<Result<OngoingTrainingReadModel>> Handle(
        GetOngoingTrainingByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var ongoingTraining = await ongoingTrainingsQuery.GetByIdAsync(
            request.Id,
            cancellationToken
        );

        if (ongoingTraining is null)
        {
            return Result.Failure<OngoingTrainingReadModel>(
                OngoingTrainingErrors.NotFoundById(request.Id)
            );
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
