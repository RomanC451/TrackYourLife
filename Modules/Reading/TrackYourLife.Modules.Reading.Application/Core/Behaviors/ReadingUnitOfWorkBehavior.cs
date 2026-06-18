using MediatR;
using TrackYourLife.Modules.Reading.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Reading.Domain.Core;
using TrackYourLife.Modules.Reading.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Reading.Application.Core.Behaviors;

internal sealed class ReadingUnitOfWorkBehavior<TRequest, TResponse>(
    IReadingUnitOfWork unitOfWork,
    IPublisher publisher,
    IReadingOutboxMessageRepository outboxMessageRepository
)
    : GenericUnitOfWorkBehavior<
        IReadingUnitOfWork,
        IReadingOutboxMessageRepository,
        TRequest,
        TResponse
    >(unitOfWork, publisher, outboxMessageRepository)
    where TRequest : class, IReadingRequest
    where TResponse : Result;
