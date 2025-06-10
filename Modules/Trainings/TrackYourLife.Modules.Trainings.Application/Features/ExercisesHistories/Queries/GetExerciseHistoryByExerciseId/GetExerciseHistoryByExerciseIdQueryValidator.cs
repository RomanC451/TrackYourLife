using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseHistoryByExerciseId;

public sealed class GetExerciseHistoryByExerciseIdQueryValidator
    : AbstractValidator<GetExerciseHistoryByExerciseIdQuery>
{
    public GetExerciseHistoryByExerciseIdQueryValidator()
    {
        RuleFor(x => x.ExerciseId).NotEmptyId();
    }
}
