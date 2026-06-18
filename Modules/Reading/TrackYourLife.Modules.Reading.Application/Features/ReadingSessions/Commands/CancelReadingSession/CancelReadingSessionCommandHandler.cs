using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.CancelReadingSession;

internal sealed class CancelReadingSessionCommandHandler(
    IReadingSessionsRepository readingSessionsRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<CancelReadingSessionCommand>
{
    public async Task<Result> Handle(
        CancelReadingSessionCommand command,
        CancellationToken cancellationToken
    )
    {
        var session = await readingSessionsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (session is null)
        {
            return Result.Failure(ReadingSessionErrors.NotFound(command.Id));
        }

        if (session.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(ReadingSessionErrors.NotOwned(command.Id));
        }

        if (!session.IsActive)
        {
            return Result.Failure(ReadingSessionErrors.SessionAlreadyFinished);
        }

        readingSessionsRepository.Remove(session);

        return Result.Success();
    }
}
