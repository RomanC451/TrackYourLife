using FluentValidation;
using TrackYourLife.Modules.Trainings.Application.Features.Exercises.Validators;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Commands.CreateExercise;

public class CreateExerciseCommandValidator : AbstractValidator<CreateExerciseCommand>
{
    public CreateExerciseCommandValidator(
        IExercisesQuery exerciseQuery,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MustAsync(
                async (name, cancellationToken) =>
                {
                    return !await exerciseQuery.ExistsByUserIdAndNameAsync(
                        userIdentifierProvider.UserId,
                        name,
                        cancellationToken
                    );
                }
            )
            .WithMessage("This name is already used.");

        RuleFor(c => c.MuscleGroups).NotEmpty();

        RuleFor(c => c.Description).MaximumLength(1000);

        RuleFor(c => c.PictureUrl).MaximumLength(500);

        RuleFor(c => c.VideoUrl).MaximumLength(500);

        RuleFor(c => c.Equipment).MaximumLength(100);

        RuleFor(c => c.Sets).NotEmpty();

        RuleForEach(c => c.Sets).SetValidator(new ExerciseSetValidator());
    }
}
