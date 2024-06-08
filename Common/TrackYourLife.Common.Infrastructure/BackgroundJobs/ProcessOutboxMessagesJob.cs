using MediatR;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using TrackYourLife.Common.Domain.OutboxMessages;
using TrackYourLife.Common.Domain.Primitives;
using TrackYourLife.Common.Domain.Repositories;

namespace TrackYourLife.Common.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob(
    IPublisher publisher,
    IOutboxMessageRepository outboxMessageRepository,
    IUnitOfWork unitOfWork,
    ILogger logger) : IJob
{
    private readonly IOutboxMessageRepository _outboxMessageRepository = outboxMessageRepository;
    private readonly IPublisher _publisher = publisher;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
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
                _logger.Error("The OutboxMessage with id: {id} couldn't be deserialized.", outboxMessage.Id);
                continue;
            }


            //TODO: publisher can fail, wrap this in a try/catch block and hadle the possible exceptions
            try
            {
                await _publisher.Publish(domainEvent, context.CancellationToken);
            }
            catch (Exception e)
            {
                _logger.Error("The domain event deserialized from the OutboxMessage with id: {id} couldn't be published.", outboxMessage.Id);
            }



            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
    }
}
