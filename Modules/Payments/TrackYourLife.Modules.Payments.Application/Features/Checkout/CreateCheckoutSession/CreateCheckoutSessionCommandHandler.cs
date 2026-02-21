using MassTransit;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Errors;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Features.Checkout.CreateCheckoutSession;

internal sealed class CreateCheckoutSessionCommandHandler(
    IStripeService stripeService,
    IUserIdentifierProvider userIdentifierProvider,
    IBus bus
) : ICommandHandler<CreateCheckoutSessionCommand, string>
{
    public async Task<Result<string>> Handle(
        CreateCheckoutSessionCommand request,
        CancellationToken cancellationToken
    )
    {
        var client = bus.CreateRequestClient<GetUserForBillingByIdRequest>();
        var response = await client.GetResponse<GetUserForBillingByIdResponse>(
            new GetUserForBillingByIdRequest(userIdentifierProvider.UserId),
            cancellationToken
        );

        if (response.Message.Errors.Count > 0)
        {
            return Result.Failure<string>(response.Message.Errors[0]);
        }

        var user = response.Message.Data;
        if (user is null)
        {
            return Result.Failure<string>(CheckoutErrors.UserNotFound);
        }

        if (user.HasActiveProSubscription)
        {
            return Result.Failure<string>(CheckoutErrors.AlreadySubscribedPro);
        }

        if (!string.IsNullOrEmpty(user.StripeCustomerId))
        {
            var alreadySubscribedForPrice =
                await stripeService.CustomerHasActiveSubscriptionForPriceAsync(
                    user.StripeCustomerId,
                    request.PriceId,
                    cancellationToken
                );
            if (alreadySubscribedForPrice)
            {
                return Result.Failure<string>(CheckoutErrors.AlreadySubscribed);
            }
        }

        var url = await stripeService.CreateCheckoutSessionAsync(
            user.StripeCustomerId,
            user.Email,
            user.UserId.Value.ToString(),
            request.PriceId,
            request.SuccessUrl,
            request.CancelUrl,
            cancellationToken
        );

        return Result.Success(url);
    }
}
