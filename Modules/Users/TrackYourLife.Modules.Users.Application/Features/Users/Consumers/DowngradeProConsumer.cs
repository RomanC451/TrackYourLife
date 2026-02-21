using MediatR;
using MassTransit;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.DowngradePro;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Consumers;

public sealed class DowngradeProConsumer(ISender sender)
    : IConsumer<DowngradeProRequest>
{
    public async Task Consume(ConsumeContext<DowngradeProRequest> context)
    {
        var result = await sender.Send(
            new DowngradeProCommand(context.Message.UserId, context.Message.SubscriptionStatus),
            context.CancellationToken
        );

        var errors = result.IsFailure ? new List<Error> { result.Error! } : new List<Error>();
        await context.RespondAsync(new DowngradeProResponse(errors));
    }
}
