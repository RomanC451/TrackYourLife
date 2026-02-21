using FluentValidation;

namespace TrackYourLife.Modules.Payments.Application.Features.BillingPortal.GetBillingPortalUrl;

internal sealed class GetBillingPortalUrlQueryValidator
    : AbstractValidator<GetBillingPortalUrlQuery>
{
    public GetBillingPortalUrlQueryValidator()
    {
        RuleFor(x => x.ReturnUrl).NotEmpty().MaximumLength(2048);
    }
}
