using MassTransit;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Consumers;

public sealed class GetUserForBillingByIdConsumer(IUserQuery userQuery, IDateTimeProvider dateTimeProvider)
    : IConsumer<GetUserForBillingByIdRequest>
{
    public async Task Consume(ConsumeContext<GetUserForBillingByIdRequest> context)
    {
        var user = await userQuery.GetByIdAsync(context.Message.UserId, context.CancellationToken);

        if (user is null)
        {
            await context.RespondAsync(
                new GetUserForBillingByIdResponse(
                    null,
                    [UserErrors.NotFound(context.Message.UserId)]
                )
            );
            return;
        }

        var hasActivePro = user.PlanType == PlanType.Pro
            && user.SubscriptionEndsAtUtc.HasValue
            && user.SubscriptionEndsAtUtc.Value > dateTimeProvider.UtcNow;

        var data = new UserForBillingDto(user.Id, user.Email, user.StripeCustomerId, hasActivePro);
        await context.RespondAsync(new GetUserForBillingByIdResponse(data, []));
    }
}
