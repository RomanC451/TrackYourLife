using FluentValidation;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetGoalByType;
using TrackYourLife.SharedLib.Domain.Enums;

namespace TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetGoalByType;

public sealed class GetGoalByTypeQueryValidator : AbstractValidator<GetGoalByTypeQuery>
{
    public GetGoalByTypeQueryValidator()
    {
        RuleFor(x => x.Type).IsInEnum();

        RuleFor(x => x.Date).NotEmpty();
    }
}
