using MassTransit;
using MediatR;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpgradeToPro;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Consumers;

public sealed class UpgradeToProConsumer(ISender sender)
    : IConsumer<UpgradeToProRequest>
{
    public async Task Consume(ConsumeContext<UpgradeToProRequest> context)
    {
        var request = context.Message;
        var result = await sender.Send(
            new UpgradeToProCommand(
                request.UserId,
                request.StripeCustomerId,
                request.PeriodEndUtc,
                request.CancelAtPeriodEnd
            ),
            context.CancellationToken
        );

        var errors = result.IsFailure ? new List<Error> { result.Error! } : new List<Error>();
        await context.RespondAsync(new UpgradeToProResponse(errors));
    }
}
