using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExerciseById;

internal sealed class GetExerciseByIdQueryValidator : AbstractValidator<GetExerciseByIdQuery>
{
    public GetExerciseByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
    }
}
