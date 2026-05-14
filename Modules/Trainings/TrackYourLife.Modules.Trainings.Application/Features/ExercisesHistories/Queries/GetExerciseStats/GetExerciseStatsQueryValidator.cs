using FluentValidation;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

namespace TrackYourLife.Modules.Trainings.Application.Features.ExercisesHistories.Queries.GetExerciseStats;

public sealed class GetExerciseStatsQueryValidator : AbstractValidator<GetExerciseStatsQuery>
{
    private static readonly int MaxCustomRangeDays = 731; // up to ~2 years

    public GetExerciseStatsQueryValidator()
    {
        RuleFor(x => x.ExerciseId)
            .Must(exerciseId => exerciseId != ExerciseId.Empty)
            .WithMessage("ExerciseId is required");

        RuleFor(x => x.Range).IsInEnum().WithMessage("Range must be a valid enum value");

        RuleFor(x => x.ChartMetric)
            .IsInEnum()
            .WithMessage("ChartMetric must be a valid enum value");

        RuleFor(x => x)
            .Must(x =>
                x.StartDate.HasValue && x.EndDate.HasValue
                || !x.StartDate.HasValue && !x.EndDate.HasValue
            )
            .WithMessage("StartDate and EndDate must both be provided or both omitted");

        When(
            x => x.StartDate.HasValue && x.EndDate.HasValue,
            () =>
            {
                RuleFor(x => x.StartDate!.Value)
                    .LessThanOrEqualTo(x => x.EndDate!.Value)
                    .WithMessage("StartDate must be less than or equal to EndDate");

                RuleFor(x => x)
                    .Must(x =>
                        x.EndDate!.Value.DayNumber - x.StartDate!.Value.DayNumber <= MaxCustomRangeDays
                    )
                    .WithMessage($"Custom date range must not exceed {MaxCustomRangeDays} days");
            }
        );
    }
}
