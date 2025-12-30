using MediatR;
using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.Modules.Youtube.Domain.Features.OutboxMessages;
using TrackYourLife.SharedLib.Application.Behaviors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Core.Behaviors;

/// <summary>
/// Represents a MediatR behavior that handles the unit of work for YouTube-related requests and responses.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
internal sealed class YoutubeUnitOfWorkBehavior<TRequest, TResponse>(
    IYoutubeUnitOfWork unitOfWork,
    IPublisher publisher,
    IYoutubeOutboxMessageRepository outboxMessageRepository
)
    : GenericUnitOfWorkBehavior<
        IYoutubeUnitOfWork,
        IYoutubeOutboxMessageRepository,
        TRequest,
        TResponse
    >(unitOfWork, publisher, outboxMessageRepository)
    where TRequest : class, IYoutubeRequest
    where TResponse : Result;

