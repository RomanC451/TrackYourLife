using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;

public class UpdateExerciseCommandHandler(
    IExercisesRepository exerciseRepository,
    IUserIdentifierProvider userIdentifierProvider,
    ISupaBaseStorage supaBaseStorage
) : ICommandHandler<UpdateExerciseCommand>
{
    public async Task<Result> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await exerciseRepository.GetByIdAsync(request.Id, cancellationToken);

        if (exercise is null)
        {
            return Result.Failure(ExercisesErrors.NotFoundById(request.Id));
        }

        if (exercise.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(ExercisesErrors.NotFoundById(request.Id));
        }

        string? newImageUrl = null;

        if (!string.IsNullOrEmpty(request.PictureUrl))
        {
            var imageUrlResult = await MoveImageToExercisesFolder(request.PictureUrl, exercise.Id);

            if (imageUrlResult.IsFailure)
            {
                return Result.Failure(imageUrlResult.Error);
            }

            newImageUrl = imageUrlResult.Value;
        }

        var result = exercise.Update(
            request.Name,
            request.MuscleGroups,
            request.Difficulty,
            request.Description,
            request.VideoUrl,
            newImageUrl,
            request.Equipment,
            request.ExerciseSets
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        exerciseRepository.Update(exercise);

        return Result.Success();
    }

    private async Task<Result<string>> MoveImageToExercisesFolder(
        string signedUrl,
        ExerciseId exerciseId
    )
    {
        if (signedUrl.Contains("sign/images/exercises/"))
        {
            return Result.Success(signedUrl);
        }
        return await supaBaseStorage.RenameFileFromSignedUrlAsync(
            signedUrl,
            $"exercises/{exerciseId.Value}.jpg"
        );
    }
}
