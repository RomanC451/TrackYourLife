using TrackYourLife.Modules.Trainings.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Commands.DeleteExerciseHistory;

public class DeleteExerciseHistoryCommandHandler(
    IExercisesHistoriesRepository exercisesHistoriesRepository,
    IExercisesRepository exercisesRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteExerciseHistoryCommand>
{
    public async Task<Result> Handle(
        DeleteExerciseHistoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var exerciseHistory = await exercisesHistoriesRepository.GetByIdAsync(
            request.ExerciseHistoryId,
            cancellationToken
        );

        if (exerciseHistory is null)
        {
            return Result.Failure(
                ExerciseHistoryErrors.NotFoundById(request.ExerciseHistoryId)
            );
        }

        var exercise = await exercisesRepository.GetByIdAsync(
            exerciseHistory.ExerciseId,
            cancellationToken
        );

        if (exercise is null || exercise.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(
                ExerciseHistoryErrors.NotOwned(request.ExerciseHistoryId)
            );
        }

        exercisesHistoriesRepository.Remove(exerciseHistory);

        return Result.Success();
    }
}
