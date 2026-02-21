using MassTransit;
using MediatR;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.UpdateProSubscriptionPeriodEnd;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Consumers;

public sealed class UpdateProSubscriptionPeriodEndConsumer(ISender sender)
    : IConsumer<UpdateProSubscriptionPeriodEndRequest>
{
    public async Task Consume(ConsumeContext<UpdateProSubscriptionPeriodEndRequest> context)
    {
        var request = context.Message;
        var result = await sender.Send(
            new UpdateProSubscriptionPeriodEndCommand(
                request.UserId,
                request.PeriodEndUtc,
                request.SubscriptionStatus,
                request.CancelAtPeriodEnd
            ),
            context.CancellationToken
        );

        var errors = result.IsFailure ? new List<Error> { result.Error! } : new List<Error>();
        await context.RespondAsync(new UpdateProSubscriptionPeriodEndResponse(errors));
    }
}
