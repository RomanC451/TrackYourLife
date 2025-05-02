using FluentValidation;
using TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;
using TrackYourLife.Modules.Users.Domain.Features.Users.ValueObjects;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.ResendVerificationEmail;

public sealed class ResendEmailVerificationCommandValidator
    : AbstractValidator<ResendEmailVerificationCommand>
{
    public ResendEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .Must(email => Email.Create(email).IsSuccess)
            .WithMessage(command => $"Invalid email: {Email.Create(command.Email).Error}");
    }
}
