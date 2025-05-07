using FluentValidation;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.LogInUser;

public class LoginUserCommandValidator : AbstractValidator<LogInUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(x => Email.Create(x).IsSuccess)
            .WithMessage(x => Email.Create(x.Email).Error.Message);
        RuleFor(x => x.Password)
            .NotEmpty()
            .Must(x => Password.Create(x).IsSuccess)
            .WithMessage(x => Password.Create(x.Password).Error.Message);

        RuleFor(x => x.DeviceId).NotEmptyId();
    }
}
