using MassTransit;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Consumers;

public sealed class GetUserForBillingByStripeCustomerIdConsumer(IUserQuery userQuery)
    : IConsumer<GetUserForBillingByStripeCustomerIdRequest>
{
    public async Task Consume(ConsumeContext<GetUserForBillingByStripeCustomerIdRequest> context)
    {
        var user = await userQuery.GetByStripeCustomerIdAsync(
            context.Message.StripeCustomerId,
            context.CancellationToken
        );

        if (user is null)
        {
            await context.RespondAsync(
                new GetUserForBillingByStripeCustomerIdResponse(
                    null,
                    [new Error("User.NotFound", "User with the given Stripe customer id was not found.", 404)]
                )
            );
            return;
        }

        var data = new UserForBillingByStripeCustomerIdDto(user.Id, user.Email);
        await context.RespondAsync(new GetUserForBillingByStripeCustomerIdResponse(data, []));
    }
}
