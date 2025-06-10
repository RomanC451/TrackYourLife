using FluentValidation;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Commands.CreateTraining;

public class CreateTrainingCommandValidator : AbstractValidator<CreateTrainingCommand>
{
    private Error NotFoundExercisesError = Error.None;

    public CreateTrainingCommandValidator(
        ITrainingsQuery trainingsQuery,
        IUserIdentifierProvider userIdentifierProvider,
        IExercisesQuery exercisesQuery
    )
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MustAsync(
                async (name, cancellationToken) =>
                {
                    return !await trainingsQuery.ExistsByUserIdAndNameAsync(
                        userIdentifierProvider.UserId,
                        name,
                        cancellationToken
                    );
                }
            )
            .WithMessage("This name is already used.");
        RuleFor(c => c.Description).MaximumLength(1000);
        RuleFor(c => c.Duration).GreaterThanOrEqualTo(0);
        RuleFor(c => c.RestSeconds).GreaterThanOrEqualTo(0);
        RuleFor(c => c.ExercisesIds)
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
