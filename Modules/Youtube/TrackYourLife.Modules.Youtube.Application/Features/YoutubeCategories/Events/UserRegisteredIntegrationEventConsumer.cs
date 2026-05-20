using MassTransit;
using MediatR;
using Serilog;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.EnsureDefaultYoutubeCategories;
using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Events;

public sealed class UserRegisteredIntegrationEventConsumer(ISender sender, ILogger logger)
    : IConsumer<UserRegisteredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredIntegrationEvent> context)
    {
        var result = await sender.Send(
            new EnsureDefaultYoutubeCategoriesCommand(context.Message.UserId),
            context.CancellationToken
        );

        if (result.IsFailure)
        {
            logger.Error(
                "Failed to seed default YouTube categories for user {UserId}: {Error}",
                context.Message.UserId,
                result.Error
            );
            throw new MessageConsumerFailedException(result.Error.ToString());
        }
    }
}
