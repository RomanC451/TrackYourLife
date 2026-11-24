using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Users.Application.Features.PersonalAccessTokens.Commands.RevokePersonalAccessToken;

public sealed class RevokePersonalAccessTokenCommandValidator
    : AbstractValidator<RevokePersonalAccessTokenCommand>
{
    public RevokePersonalAccessTokenCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
    }
}
