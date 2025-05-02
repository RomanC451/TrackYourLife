using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogOutUser;

public sealed class LogOutUserCommandValidator : AbstractValidator<LogOutUserCommand>
{
    public LogOutUserCommandValidator()
    {
        RuleFor(x => x.DeviceId).NotEmptyId();
    }
}
