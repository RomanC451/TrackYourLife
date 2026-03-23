using FluentValidation;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.CreateWorkoutPlan;

public sealed class CreateWorkoutPlanCommandValidator : AbstractValidator<CreateWorkoutPlanCommand>
{
    private Error NotFoundTrainingError = Error.None;

    public CreateWorkoutPlanCommandValidator(ITrainingsQuery trainingsQuery)
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.TrainingIds)
            .NotEmpty()
            .ForEach(e => e.NotEmptyId())
            .Must(ids => ids.Distinct().Count() == ids.Count())
            .WithMessage("Training ids must be unique.")
            .MustAsync(
                async (ids, cancellationToken) =>
                    await AllTrainingsExistAsync(ids.ToList(), trainingsQuery, cancellationToken)
            )
            .WithMessage(_ => NotFoundTrainingError.Message);
    }

    private async Task<bool> AllTrainingsExistAsync(
        List<TrainingId> trainingIds,
        ITrainingsQuery trainingsQuery,
        CancellationToken cancellationToken
    )
    {
        foreach (var trainingId in trainingIds)
        {
            var training = await trainingsQuery.GetByIdAsync(trainingId, cancellationToken);
            if (training is null)
            {
                NotFoundTrainingError = TrainingsErrors.NotFoundById(trainingId);
                return false;
            }
        }

        return true;
    }
}
