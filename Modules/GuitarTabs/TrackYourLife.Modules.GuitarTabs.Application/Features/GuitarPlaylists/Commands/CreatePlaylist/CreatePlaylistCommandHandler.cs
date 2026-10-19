using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.CreatePlaylist;

internal sealed class CreatePlaylistCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarPlaylistsRepository guitarPlaylistsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreatePlaylistCommand, GuitarPlaylistId>
{
    public async Task<Result<GuitarPlaylistId>> Handle(
        CreatePlaylistCommand request,
        CancellationToken cancellationToken
    )
    {
        var id = GuitarPlaylistId.NewId();
        var createResult = GuitarPlaylist.Create(
            id,
            userIdentifierProvider.UserId,
            request.Name,
            dateTimeProvider.UtcNow
        );

        if (createResult.IsFailure)
        {
            return Result.Failure<GuitarPlaylistId>(createResult.Error);
        }

        await guitarPlaylistsRepository.AddAsync(createResult.Value, cancellationToken);

        return Result.Success(id);
    }
}
