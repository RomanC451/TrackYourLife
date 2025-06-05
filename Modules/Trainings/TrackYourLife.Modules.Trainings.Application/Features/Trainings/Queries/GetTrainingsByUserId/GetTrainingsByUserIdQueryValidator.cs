using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.Trainings.Queries.GetTrainingsByUserId;

public class GetTrainingsByUserIdQueryValidator : AbstractValidator<GetTrainingsByUserIdQuery>
{
    public GetTrainingsByUserIdQueryValidator()
    {
        // No validation rules needed as the query has no parameters
    }
}
