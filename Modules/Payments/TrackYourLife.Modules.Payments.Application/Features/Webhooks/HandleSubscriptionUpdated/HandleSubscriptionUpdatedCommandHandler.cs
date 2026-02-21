using MassTransit;
using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Errors;
using TrackYourLife.SharedLib.Contracts;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Features.Webhooks.HandleSubscriptionUpdated;

internal sealed class HandleSubscriptionUpdatedCommandHandler(IBus bus)
    : ICommandHandler<HandleSubscriptionUpdatedCommand>
{
    public async Task<Result> Handle(
        HandleSubscriptionUpdatedCommand request,
        CancellationToken cancellationToken
    )
    {
        var payload = request.Payload;

        if (string.IsNullOrEmpty(payload.CustomerId))
        {
            return Result.Failure(StripeWebhookErrors.SubscriptionUpdated.MissingCustomerId);
        }

        var subscriptionStatus = SubscriptionStatusMapping.Parse(payload.SubscriptionStatus);
        if (subscriptionStatus is null)
        {
            return Result.Failure(
                StripeWebhookErrors.SubscriptionUpdated.InvalidSubscriptionStatus
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
            return Result.Failure(StripeWebhookErrors.SubscriptionUpdated.UserNotFound);
        }

        var user = userResponse.Message.Data;

        if (subscriptionStatus == SubscriptionStatus.Active && payload.CurrentPeriodEndUtc != null)
        {
            var updateClient = bus.CreateRequestClient<UpdateProSubscriptionPeriodEndRequest>();
            var updateResponse =
                await updateClient.GetResponse<UpdateProSubscriptionPeriodEndResponse>(
                    new UpdateProSubscriptionPeriodEndRequest(
                        user.UserId,
                        payload.CurrentPeriodEndUtc.Value,
                        subscriptionStatus.Value,
                        payload.CancelAtPeriodEnd
                    ),
                    cancellationToken
                );
            if (updateResponse.Message.Errors.Count > 0)
            {
                return Result.Failure(updateResponse.Message.Errors[0]);
            }

            return Result.Success();
        }

        if (
            subscriptionStatus
            is SubscriptionStatus.Canceled
                or SubscriptionStatus.Unpaid
                or SubscriptionStatus.PastDue
        )
        {
            var downgradeClient = bus.CreateRequestClient<DowngradeProRequest>();
            var downgradeResponse = await downgradeClient.GetResponse<DowngradeProResponse>(
                new DowngradeProRequest(user.UserId, subscriptionStatus.Value),
                cancellationToken
            );
            if (downgradeResponse.Message.Errors.Count > 0)
            {
                return Result.Failure(downgradeResponse.Message.Errors[0]);
            }

            return Result.Success();
        }

        return Result.Success();
    }
}
