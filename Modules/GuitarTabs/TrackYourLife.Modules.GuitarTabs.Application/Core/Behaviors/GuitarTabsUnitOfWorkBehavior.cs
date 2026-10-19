using MediatR;
using TrackYourLife.Modules.GuitarTabs.Domain.Core;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.GuitarTabs.Application.Core.Behaviors;

internal sealed class GuitarTabsUnitOfWorkBehavior<TRequest, TResponse>(
    IGuitarTabsUnitOfWork unitOfWork,
    IPublisher publisher,
    IGuitarTabsOutboxMessageRepository outboxMessageRepository
)
    : GenericUnitOfWorkBehavior<
        IGuitarTabsUnitOfWork,
        IGuitarTabsOutboxMessageRepository,
        TRequest,
        TResponse
    >(unitOfWork, publisher, outboxMessageRepository)
    where TRequest : class, IGuitarTabsRequest
    where TResponse : Result;
