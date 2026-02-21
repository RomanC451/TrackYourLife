using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Payments.Application.Features.BillingPortal.GetBillingPortalUrl;

public sealed record GetBillingPortalUrlQuery(string ReturnUrl) : IQuery<string>;
