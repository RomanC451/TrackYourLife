using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.CreateExercise;

public sealed class CreateExerciseCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IExerciseRepository exercisesRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateExerciseCommand, ExerciseId>
{
    public async Task<Result<ExerciseId>> Handle(
        CreateExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var exercise = Exercise.Create(
            ExerciseId.NewId(),
            userIdentifierProvider.UserId,
            request.Name,
            request.PictureUrl,
            request.VideoUrl,
            request.Description,
            request.Equipment,
            request.Sets,
            dateTimeProvider.UtcNow
        );

        if (exercise.IsFailure)
        {
            return Result.Failure<ExerciseId>(exercise.Error);
        }

        await exercisesRepository.AddAsync(exercise.Value, cancellationToken);

        return Result.Success(exercise.Value.Id);
    }
}
