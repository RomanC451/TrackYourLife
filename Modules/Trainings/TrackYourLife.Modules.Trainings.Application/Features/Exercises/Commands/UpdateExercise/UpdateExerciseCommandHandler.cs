using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;

public class UpdateExerciseCommandHandler(
    IExerciseRepository exerciseRepository,
    IUserIdentifierProvider userIdentifierProvider,
    IDateTimeProvider dateTimeProvider
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

        var result = exercise.Update(
            request.Name,
            request.Description,
            request.VideoUrl,
            request.PictureUrl,
            request.Equipment,
            request.ExerciseSets,
            dateTimeProvider.UtcNow
        );

        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        exerciseRepository.Update(exercise);

        return Result.Success();
    }
}
