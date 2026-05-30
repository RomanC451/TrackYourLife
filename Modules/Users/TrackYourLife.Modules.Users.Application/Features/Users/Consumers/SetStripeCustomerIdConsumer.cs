using MassTransit;
using MediatR;
using TrackYourLife.Modules.Users.Application.Features.Users.Commands.SetStripeCustomerId;
using TrackYourLife.SharedLib.Contracts.Integration.Users;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Consumers;

public sealed class SetStripeCustomerIdConsumer(ISender sender)
    : IConsumer<SetStripeCustomerIdRequest>
{
    public async Task Consume(ConsumeContext<SetStripeCustomerIdRequest> context)
    {
        var request = context.Message;
        var result = await sender.Send(
            new SetStripeCustomerIdCommand(request.UserId, request.StripeCustomerId),
            context.CancellationToken
        );

        var errors = result.IsFailure ? new List<Error> { result.Error! } : new List<Error>();
        await context.RespondAsync(new SetStripeCustomerIdResponse(errors));
    }
}
