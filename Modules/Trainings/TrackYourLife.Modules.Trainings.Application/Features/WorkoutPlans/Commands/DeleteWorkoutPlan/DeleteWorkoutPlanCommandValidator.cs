using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.WorkoutPlans.Commands.DeleteWorkoutPlan;

public sealed class DeleteWorkoutPlanCommandValidator : AbstractValidator<DeleteWorkoutPlanCommand>
{
    public DeleteWorkoutPlanCommandValidator()
    {
        RuleFor(x => x.WorkoutPlanId).NotEmptyId();
    }
}
