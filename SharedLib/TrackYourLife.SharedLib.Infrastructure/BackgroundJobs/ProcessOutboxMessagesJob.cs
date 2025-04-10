using MediatR;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using TrackYourLife.SharedLib.Domain.OutboxMessages;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Repositories;

namespace TrackYourLife.SharedLib.Infrastructure.BackgroundJobs;

/// <summary>
/// Background job that processes outbox messages by publishing domain events.
/// This job ensures reliable event publishing by retrying failed messages.
/// </summary>
public class ProcessOutboxMessagesJob(
    IPublisher publisher,
    IOutboxMessageRepository outboxMessageRepository,
    IUnitOfWork unitOfWork,
    ILogger logger
) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            List<OutboxMessage> messages =
                await outboxMessageRepository.GetUnprocessedMessagesAsync(
                    context.CancellationToken
                );

            if (messages.Count == 0)
            {
                logger.Debug("No unprocessed messages found at {Time}", DateTime.UtcNow);
                return;
            }

            logger.Information(
                "Processing {Count} messages at {Time}",
                messages.Count,
                DateTime.UtcNow
            );

            foreach (OutboxMessage outboxMessage in messages)
            {
                try
                {
                    IOutboxDomainEvent? domainEvent =
                        JsonConvert.DeserializeObject<IOutboxDomainEvent>(
                            outboxMessage.Content,
                            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
                        );

                    if (domainEvent is null)
                    {
                        logger.Error(
                            "Failed to deserialize OutboxMessage with id: {Id}. Content: {Content}",
                            outboxMessage.Id,
                            outboxMessage.Content
                        );
                        continue;
                    }

                    await publisher.Publish(domainEvent, context.CancellationToken);

                    outboxMessage.ProcessedOnUtc = DateTime.UtcNow;

                    logger.Information(
                        "Successfully processed message {MessageId} of type {EventType} at {Time}",
                        outboxMessage.Id,
                        domainEvent.GetType().Name,
                        DateTime.UtcNow
                    );
                }
                catch (Exception e)
                {
                    logger.Error(
                        e,
                        "Failed to process OutboxMessage with id: {Id}. Content: {Content}",
                        outboxMessage.Id,
                        outboxMessage.Content
                    );
                    // Continue processing other messages even if one fails
                }
            }

            await unitOfWork.SaveChangesAsync(context.CancellationToken);
            logger.Information("Completed processing messages at {Time}", DateTime.UtcNow);
        }
        catch (Exception e)
        {
            logger.Error(e, "Failed to execute ProcessOutboxMessagesJob");
            throw new JobExecutionException("Failed to process outbox messages", e, false);
        }
    }
}
