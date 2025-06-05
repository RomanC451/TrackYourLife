using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.Exercises.Queries.GetExercisesByUserId;

public class GetExercisesByUserIdQueryValidator : AbstractValidator<GetExercisesByUserIdQuery>
{
    public GetExercisesByUserIdQueryValidator()
    {
        // No validation rules needed as the query has no parameters
    }
}
