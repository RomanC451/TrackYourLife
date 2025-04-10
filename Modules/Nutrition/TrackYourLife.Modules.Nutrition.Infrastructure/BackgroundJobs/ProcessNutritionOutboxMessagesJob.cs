using MediatR;
using Quartz;
using Serilog;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Infrastructure.BackgroundJobs;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.BackgroundJobs;

[DisallowConcurrentExecution]
internal sealed class ProcessNutritionOutboxMessagesJob(
    IPublisher publisher,
    INutritionOutboxMessageRepository outboxMessageRepository,
    INutritionUnitOfWork unitOfWork,
    ILogger logger
) : ProcessOutboxMessagesJob(publisher, outboxMessageRepository, unitOfWork, logger);
