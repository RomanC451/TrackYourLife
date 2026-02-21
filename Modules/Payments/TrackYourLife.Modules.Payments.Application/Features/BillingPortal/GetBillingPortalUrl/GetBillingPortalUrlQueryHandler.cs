using MassTransit;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Errors;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Features.BillingPortal.GetBillingPortalUrl;

internal sealed class GetBillingPortalUrlQueryHandler(
    IStripeService stripeService,
    IUserIdentifierProvider userIdentifierProvider,
    IBus bus
) : IQueryHandler<GetBillingPortalUrlQuery, string>
{
    public async Task<Result<string>> Handle(
        GetBillingPortalUrlQuery request,
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
            return Result.Failure<string>(BillingPortalErrors.UserNotFound);
        }

        if (string.IsNullOrEmpty(user.StripeCustomerId))
        {
            return Result.Failure<string>(BillingPortalErrors.NoStripeCustomer);
        }

        var url = await stripeService.CreateBillingPortalSessionAsync(
            user.StripeCustomerId,
            request.ReturnUrl,
            cancellationToken
        );

        return Result.Success(url);
    }
}
