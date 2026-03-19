using MassTransit;
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
    IUserIdentifierProvider userIdentifierProvider,
    IBus bus
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

        if (string.IsNullOrEmpty(user.StripeCustomerId))
        {
            return Result.Failure<BillingSummaryDto>(BillingSummaryErrors.NoStripeCustomer);
        }

        var summary = await stripeService.GetBillingSummaryAsync(
            user.StripeCustomerId,
            cancellationToken
        );

        return Result.Success(summary);
    }
}

