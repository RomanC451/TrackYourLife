using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Trainings.Application.Features.OngoingTrainings.Queries.GetOngoingTrainingById;

public class GetOngoingTrainingByIdQueryValidator : AbstractValidator<GetOngoingTrainingByIdQuery>
{
    public GetOngoingTrainingByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
    }
}
