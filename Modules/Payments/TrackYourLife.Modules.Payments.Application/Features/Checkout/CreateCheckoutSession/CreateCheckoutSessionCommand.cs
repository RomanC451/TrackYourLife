using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;

namespace TrackYourLife.Modules.Payments.Application.Features.Checkout.CreateCheckoutSession;

public sealed record CreateCheckoutSessionCommand(
    string SuccessUrl,
    string CancelUrl,
    string PriceId
) : ICommand<string>;
