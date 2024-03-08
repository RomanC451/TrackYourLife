using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using TrackYourLifeDotnet.Domain.Primitives;
using TrackYourLifeDotnet.Persistence;
using TrackYourLifeDotnet.Persistence.Outbox;

namespace TrackYourLifeDotnet.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
public class ProcessOutboxMessagesJob : IJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPublisher _publisher;

    public ProcessOutboxMessagesJob(IPublisher publisher, ApplicationDbContext dbContext)
    {
        _publisher = publisher;
        _dbContext = dbContext;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        List<OutboxMessage> messages = await _dbContext
            .Set<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(context.CancellationToken);

        foreach (OutboxMessage outboxMessage in messages)
        {
            IDomainEvent? domainEvent = null;
            domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(
                outboxMessage.Content,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All }
            );

            // if (domainEvent is null)
            // {
            //     //TODO: to be loged somewhere
            //     continue;
            // }


            //TODO: publisher can fail, wrap this in a try/catch block and hadle the possible exceptions
            await _publisher.Publish(domainEvent!, context.CancellationToken);

            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();
    }
}
