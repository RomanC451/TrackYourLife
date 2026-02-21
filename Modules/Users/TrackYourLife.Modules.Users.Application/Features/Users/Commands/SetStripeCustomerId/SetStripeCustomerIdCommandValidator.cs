using FluentValidation;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Commands.SetStripeCustomerId;

internal sealed class SetStripeCustomerIdCommandValidator
    : AbstractValidator<SetStripeCustomerIdCommand>
{
    public SetStripeCustomerIdCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.StripeCustomerId).NotEmpty().MaximumLength(256);
    }
}
