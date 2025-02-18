using MediatR;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.BackgroundJobs;

// [DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob(
    IPublisher publisher,
    INutritionOutboxMessageRepository outboxMessageRepository,
    INutritionUnitOfWork unitOfWork,
    ILogger logger
) : IJob
{
    private readonly INutritionOutboxMessageRepository _outboxMessageRepository =
        outboxMessageRepository;
    private readonly IPublisher _publisher = publisher;
    private readonly INutritionUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger _logger = logger;

    public async Task Execute(IJobExecutionContext context)
    {
        List<OutboxMessage> messages = await _outboxMessageRepository.GetUnprocessedMessagesAsync(
            context.CancellationToken
        );

        foreach (OutboxMessage outboxMessage in messages)
        {
            IDomainEvent? domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                outboxMessage.Content,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
            );

            if (domainEvent is null)
            {
                _logger.Error(
                    "The OutboxMessage with id: {Id} couldn't be deserialized.",
                    outboxMessage.Id
                );
                continue;
            }

            // TODO: publisher can fail, wrap this in a try/catch block and handle the possible exceptions
            try
            {
                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error(
                    e,
                    "The domain event deserialized from the OutboxMessage with id: {Id} couldn't be published.",
                    outboxMessage.Id
                );
            }

            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}
