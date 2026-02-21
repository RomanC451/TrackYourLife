using MassTransit;
using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Errors;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleCheckoutSessionCompleted;

internal sealed class HandleCheckoutSessionCompletedCommandHandler(IBus bus)
    : ICommandHandler<HandleCheckoutSessionCompletedCommand>
{
    public async Task<Result> Handle(
        HandleCheckoutSessionCompletedCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        if (payload is null)
        {
            return Result.Failure(StripeWebhookErrors.CheckoutSessionCompleted.MissingPayload);
        }

        if (string.IsNullOrEmpty(payload.ClientReferenceId) || !Guid.TryParse(payload.ClientReferenceId, out var userIdValue))
        {
            return Result.Failure(StripeWebhookErrors.CheckoutSessionCompleted.InvalidClientReferenceId);
        }

        if (string.IsNullOrEmpty(payload.CustomerId))
        {
            return Result.Failure(StripeWebhookErrors.CheckoutSessionCompleted.MissingCustomerId);
        }

        if (payload.CurrentPeriodEndUtc is null)
        {
            return Result.Failure(StripeWebhookErrors.CheckoutSessionCompleted.MissingCurrentPeriodEnd);
        }

        var userId = new UserId(userIdValue);
        var client = bus.CreateRequestClient<UpgradeToProRequest>();
        var response = await client.GetResponse<UpgradeToProResponse>(
            new UpgradeToProRequest(
                userId,
                payload.CustomerId,
                payload.CurrentPeriodEndUtc.Value,
                payload.CancelAtPeriodEnd
            ),
            cancellationToken
        );

        if (response.Message.Errors.Count > 0)
        {
            return Result.Failure(response.Message.Errors[0]);
        }

        return Result.Success();
    }
}
