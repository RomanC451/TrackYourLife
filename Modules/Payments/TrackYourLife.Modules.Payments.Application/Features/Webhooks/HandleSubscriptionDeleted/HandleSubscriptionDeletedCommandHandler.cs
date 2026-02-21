using MassTransit;
using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Errors;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionDeleted;

internal sealed class HandleSubscriptionDeletedCommandHandler(IBus bus)
    : ICommandHandler<HandleSubscriptionDeletedCommand>
{
    public async Task<Result> Handle(
        HandleSubscriptionDeletedCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        if (string.IsNullOrEmpty(payload.CustomerId))
        {
            return Result.Failure(StripeWebhookErrors.SubscriptionDeleted.MissingCustomerId);
        }

        var subscriptionStatus = SubscriptionStatusMapping.Parse(payload.SubscriptionStatus);
        if (subscriptionStatus is null)
        {
            return Result.Failure(
                StripeWebhookErrors.SubscriptionDeleted.InvalidSubscriptionStatus
            );
        }

        var userClient = bus.CreateRequestClient<GetUserForBillingByStripeCustomerIdRequest>();
        var userResponse =
            await userClient.GetResponse<GetUserForBillingByStripeCustomerIdResponse>(
                new GetUserForBillingByStripeCustomerIdRequest(payload.CustomerId),
                cancellationToken
            );

        if (userResponse.Message.Errors.Count > 0)
        {
            return Result.Failure(userResponse.Message.Errors[0]);
        }

        if (userResponse.Message.Data is null)
        {
            return Result.Failure(StripeWebhookErrors.SubscriptionDeleted.UserNotFound);
        }

        var downgradeClient = bus.CreateRequestClient<DowngradeProRequest>();
        var downgradeResponse = await downgradeClient.GetResponse<DowngradeProResponse>(
            new DowngradeProRequest(userResponse.Message.Data.UserId, subscriptionStatus.Value),
            cancellationToken
        );

        if (downgradeResponse.Message.Errors.Count > 0)
        {
            return Result.Failure(downgradeResponse.Message.Errors[0]);
        }

        return Result.Success();
    }
}
