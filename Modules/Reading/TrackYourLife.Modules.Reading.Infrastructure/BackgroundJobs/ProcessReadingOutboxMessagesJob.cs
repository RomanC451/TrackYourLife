using MediatR;
using Quartz;
using Serilog;
using TrackYourLife.Modules.Reading.Domain.Core;
using TrackYourLife.Modules.Reading.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.BackgroundJobs;

namespace TrackYourLife.Modules.Reading.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessReadingOutboxMessagesJob(
    IPublisher publisher,
    IReadingOutboxMessageRepository outboxMessageRepository,
    IReadingUnitOfWork unitOfWork,
    ILogger logger
) : ProcessOutboxMessagesJob(publisher, outboxMessageRepository, unitOfWork, logger);
