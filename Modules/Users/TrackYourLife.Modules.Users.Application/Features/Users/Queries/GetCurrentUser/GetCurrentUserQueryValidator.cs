using FluentValidation;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryValidator : AbstractValidator<GetCurrentUserQuery>
{
    public GetCurrentUserQueryValidator()
    {
        // No validation rules needed as the query has no parameters
    }
}
