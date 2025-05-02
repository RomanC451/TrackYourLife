using FluentValidation;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RefreshJwtToken;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.RefreshJwtToken;

public sealed class RefreshJwtTokenCommandValidator : AbstractValidator<RefreshJwtTokenCommand>
{
    public RefreshJwtTokenCommandValidator()
    {
        RuleFor(x => x.RefreshTokenValue).NotEmpty();

        RuleFor(x => x.DeviceId).NotEmptyId();
    }
}
