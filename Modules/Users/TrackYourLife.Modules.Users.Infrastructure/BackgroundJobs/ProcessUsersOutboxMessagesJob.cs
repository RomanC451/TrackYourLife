using MediatR;
using Quartz;
using Serilog;
using TrackYourLife.Modules.Users.Domain.Core;
using TrackYourLife.Modules.Users.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.BackgroundJobs;

namespace TrackYourLife.Modules.Users.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessUsersOutboxMessagesJob(
    IPublisher publisher,
    IUsersOutboxMessageRepository outboxMessageRepository,
    IUsersUnitOfWork unitOfWork,
    ILogger logger
) : ProcessOutboxMessagesJob(publisher, outboxMessageRepository, unitOfWork, logger);
