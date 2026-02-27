using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Commands.DeleteExerciseHistory;

public class DeleteExerciseHistoryCommandValidator : AbstractValidator<DeleteExerciseHistoryCommand>
{
    public DeleteExerciseHistoryCommandValidator()
    {
        RuleFor(command => command.ExerciseHistoryId).NotEmptyId();
    }
}
