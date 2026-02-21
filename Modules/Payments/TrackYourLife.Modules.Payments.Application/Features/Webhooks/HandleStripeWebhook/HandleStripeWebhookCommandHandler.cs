using MediatR;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Errors;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleCheckoutSessionCompleted;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionDeleted;
using TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionUpdated;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleStripeWebhook;

internal sealed class HandleStripeWebhookCommandHandler(
    IStripeService stripeService,
    ISender sender
) : ICommandHandler<HandleStripeWebhookCommand>
{
    public async Task<Result> Handle(
        HandleStripeWebhookCommand request,
        CancellationToken cancellationToken
    )
    {
        var (payload, error) = stripeService.TryParseWebhookEvent(
            request.JsonPayload,
            request.SignatureHeader
        );

        if (error != null)
        {
            return Result.Failure(StripeWebhookErrors.ParseFailed(error));
        }

        if (payload == null)
        {
            return Result.Failure(StripeWebhookErrors.PayloadNull);
        }

        if (await stripeService.HasProcessedEventAsync(payload.EventId, cancellationToken))
        {
            return Result.Success();
        }

        Result result = payload.EventType switch
        {
            "checkout.session.completed" => await sender.Send(
                new HandleCheckoutSessionCompletedCommand(payload),
                cancellationToken
            ),
            "customer.subscription.updated" => await sender.Send(
                new HandleSubscriptionUpdatedCommand(payload),
                cancellationToken
            ),
            "customer.subscription.deleted" => await sender.Send(
                new HandleSubscriptionDeletedCommand(payload),
                cancellationToken
            ),
            "invoice.payment_failed" => Result.Success(),
            _ => Result.Success(),
        };

        if (result.IsSuccess)
        {
            await stripeService.MarkEventProcessedAsync(payload.EventId, cancellationToken);
        }

        return result;
    }
}
