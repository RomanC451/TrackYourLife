using MediatR;
using Quartz;
using Serilog;
using TrackYourLife.Modules.GuitarTabs.Domain.Core;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.BackgroundJobs;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessGuitarTabsOutboxMessagesJob(
    IPublisher publisher,
    IGuitarTabsOutboxMessageRepository outboxMessageRepository,
    IGuitarTabsUnitOfWork unitOfWork,
    ILogger logger
) : ProcessOutboxMessagesJob(publisher, outboxMessageRepository, unitOfWork, logger);
