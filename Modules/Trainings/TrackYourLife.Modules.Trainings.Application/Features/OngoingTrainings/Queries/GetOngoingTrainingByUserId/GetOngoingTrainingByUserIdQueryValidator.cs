using FluentValidation;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingByUserId;

public class GetOngoingTrainingByUserIdQueryValidator
    : AbstractValidator<GetOngoingTrainingByUserIdQuery>
{
    public GetOngoingTrainingByUserIdQueryValidator()
    {
        // Nothing to validate
    }
}
