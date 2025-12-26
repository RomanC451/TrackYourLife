using FluentValidation;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Validators;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Application.Extensions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.UpdateExercise;

public sealed class UpdateExerciseCommandValidator : AbstractValidator<UpdateExerciseCommand>
{
    public UpdateExerciseCommandValidator(
        IExercisesQuery exerciseQuery,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        RuleFor(c => c.Id).NotEmptyId();

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MustAsync(
                (model, name, cancellationToken) =>
                    IsNameUniqueAsync(
                        model.Id,
                        name,
                        userIdentifierProvider.UserId,
                        exerciseQuery,
                        cancellationToken
                    )
            )
            .WithMessage("This name is already used.");

        RuleFor(c => c.MuscleGroups).NotEmpty();

        RuleFor(c => c.Description).MaximumLength(1000);

        RuleFor(c => c.PictureUrl).MaximumLength(500);

        RuleFor(c => c.VideoUrl).MaximumLength(500);

        RuleFor(c => c.Equipment).MaximumLength(100);

        RuleFor(c => c.ExerciseSets).NotEmpty();
        RuleForEach(c => c.ExerciseSets).SetValidator(new ExerciseSetValidator());
    }

    private static async Task<bool> IsNameUniqueAsync(
        ExerciseId id,
        string name,
        UserId userId,
        IExercisesQuery exerciseQuery,
        CancellationToken cancellationToken
    )
    {
        var existingExercise = await exerciseQuery.GetByUserIdAndNameAsync(
            userId,
            name,
            cancellationToken
        );

        return existingExercise == null || existingExercise.Id == id;
    }
}
