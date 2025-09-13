using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.CreateExercise;

public sealed class CreateExerciseCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IExercisesRepository exercisesRepository,
    ISupaBaseStorage supaBaseStorage,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateExerciseCommand, ExerciseId>
{
    public async Task<Result<ExerciseId>> Handle(
        CreateExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var newImageUrl = string.Empty;

        var exerciseId = ExerciseId.NewId();

        if (!string.IsNullOrEmpty(request.PictureUrl))
        {
            var imageUrlResult = await MoveImageToExercisesFolder(request.PictureUrl, exerciseId);

            if (imageUrlResult.IsFailure)
            {
                return Result.Failure<ExerciseId>(imageUrlResult.Error);
            }

            newImageUrl = imageUrlResult.Value;
        }

        var exercise = Exercise.Create(
            id: exerciseId,
            userId: userIdentifierProvider.UserId,
            name: request.Name,
            muscleGroups: request.MuscleGroups,
            difficulty: request.Difficulty,
            pictureUrl: newImageUrl,
            videoUrl: request.VideoUrl,
            description: request.Description,
            equipment: request.Equipment,
            sets: request.Sets,
            createdOn: dateTimeProvider.UtcNow
        );

        if (exercise.IsFailure)
        {
            return Result.Failure<ExerciseId>(exercise.Error);
        }

        await exercisesRepository.AddAsync(exercise.Value, cancellationToken);

        return Result.Success(exercise.Value.Id);
    }

    private async Task<Result<string>> MoveImageToExercisesFolder(
        string signedUrl,
        ExerciseId exerciseId
    )
    {
        var result = await supaBaseStorage.RenameFileFromSignedUrlAsync(
            signedUrl,
            $"exercises/{exerciseId.Value}.jpg"
        );

        return result;
    }
}
