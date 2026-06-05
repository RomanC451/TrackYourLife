using FluentValidation;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingStats;

public sealed class GetTrainingStatsQueryValidator : AbstractValidator<GetTrainingStatsQuery>
{
    private const int MaxCustomRangeDays = 731;

    public GetTrainingStatsQueryValidator()
    {
        RuleFor(x => x.TrainingId)
            .Must(trainingId => trainingId != TrainingId.Empty)
            .WithMessage("TrainingId is required");

        RuleFor(x => x.Range).IsInEnum().WithMessage("Range must be a valid enum value");

        RuleFor(x => x.ChartAggregationType)
            .IsInEnum()
            .WithMessage("ChartAggregationType must be a valid enum value");

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
                        x.EndDate!.Value.DayNumber - x.StartDate!.Value.DayNumber
                        <= MaxCustomRangeDays
                    )
                    .WithMessage($"Custom date range must not exceed {MaxCustomRangeDays} days");
            }
        );
    }
}
