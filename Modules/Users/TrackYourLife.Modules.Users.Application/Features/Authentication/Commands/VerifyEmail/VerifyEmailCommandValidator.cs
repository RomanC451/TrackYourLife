using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Authentication.Commands.VerifyEmail;

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.VerificationToken).NotEmpty();
    }
}
