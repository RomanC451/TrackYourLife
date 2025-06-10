using FluentValidation;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.UpdateTraining;

public class UpdateTrainingCommandValidator : AbstractValidator<UpdateTrainingCommand>
{
    private Error NotFoundExercisesError = Error.None;

    public UpdateTrainingCommandValidator(IExercisesQuery exercisesQuery)
    {
        RuleFor(x => x.TrainingId).NotEmptyId();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Duration).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RestSeconds).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.ExerciseIds)
            .NotEmpty()
            .ForEach(e => e.NotEmptyId())
            .MustAsync(
                async (e, cancellationToken) =>
                    await AllExercisesExistAsync(e.ToList(), exercisesQuery, cancellationToken)
            )
            .WithMessage(c => NotFoundExercisesError.Message);
    }

    private async Task<bool> AllExercisesExistAsync(
        List<ExerciseId> exercisesIds,
        IExercisesQuery exerciseQuery,
        CancellationToken cancellationToken
    )
    {
        var exercises = await exerciseQuery.GetEnumerableWithinIdsCollectionAsync(
            exercisesIds,
            cancellationToken
        );

        var notFoundExercises = exercisesIds.Where(id => !exercises.Any(e => e.Id == id));

        if (notFoundExercises.Any())
        {
            NotFoundExercisesError = ExercisesErrors.NotFoundById(notFoundExercises.First());
            return false;
        }

        return true;
    }
}
