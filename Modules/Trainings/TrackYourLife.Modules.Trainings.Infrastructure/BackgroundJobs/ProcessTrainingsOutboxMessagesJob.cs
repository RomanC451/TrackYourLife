using MediatR;
using Quartz;
using Serilog;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.BackgroundJobs;

namespace TrackYourLife.Modules.Trainings.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessTrainingsOutboxMessagesJob(
    IPublisher publisher,
    ITrainingsOutboxMessageRepository outboxMessageRepository,
    ITrainingsUnitOfWork unitOfWork,
    ILogger logger
) : ProcessOutboxMessagesJob(publisher, outboxMessageRepository, unitOfWork, logger);
