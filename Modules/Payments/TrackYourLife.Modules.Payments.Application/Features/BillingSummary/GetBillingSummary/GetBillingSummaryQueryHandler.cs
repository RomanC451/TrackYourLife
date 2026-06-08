using MassTransit;
using Microsoft.Extensions.Configuration;
using TrackYourLife.Modules.Payments.Application.Abstraction;
using TrackYourLife.Modules.Payments.Application.Contracts;
using TrackYourLife.Modules.Payments.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Payments.Application.Errors;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Payments.Application.Features.BillingSummary.GetBillingSummary;

internal sealed class GetBillingSummaryQueryHandler(
    IStripeService stripeService,
    IStripeCustomerIdResolver stripeCustomerIdResolver,
    IUserIdentifierProvider userIdentifierProvider,
    IBus bus,
    IConfiguration configuration
) : IQueryHandler<GetBillingSummaryQuery, BillingSummaryDto>
{
    public async Task<Result<BillingSummaryDto>> Handle(
        GetBillingSummaryQuery request,
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
            return Result.Failure<BillingSummaryDto>(response.Message.Errors[0]);
        }

        var user = response.Message.Data;
        if (user is null)
        {
            return Result.Failure<BillingSummaryDto>(BillingSummaryErrors.UserNotFound);
        }

        var useE2eMocks = configuration.GetValue<bool>("FeatureFlags:UseE2eMocks");
        if (string.IsNullOrEmpty(user.StripeCustomerId) && !useE2eMocks)
        {
            return Result.Failure<BillingSummaryDto>(BillingSummaryErrors.NoStripeCustomer);
        }

        var customerIdResult = await stripeCustomerIdResolver.ResolveAndPersistAsync(
            user.UserId,
            user.StripeCustomerId,
            user.Email,
            cancellationToken
        );
        if (customerIdResult.IsFailure)
        {
            return Result.Failure<BillingSummaryDto>(customerIdResult.Error!);
        }

        var summary = await stripeService.GetBillingSummaryAsync(
            customerIdResult.Value,
            cancellationToken
        );

        return Result.Success(summary);
    }
}
