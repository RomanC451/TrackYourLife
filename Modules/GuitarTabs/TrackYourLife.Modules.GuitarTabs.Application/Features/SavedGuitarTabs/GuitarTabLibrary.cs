using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;

internal sealed class GuitarTabLibrary(
    IUserIdentifierProvider userIdentifierProvider,
    IDateTimeProvider dateTimeProvider,
    ISavedGuitarTabsRepository savedGuitarTabsRepository,
    IGuitarPlaylistSongsRepository guitarPlaylistSongsRepository,
    IGuitarPlaylistsRepository guitarPlaylistsRepository
) : IGuitarTabLibrary
{
    public async Task<Result<GuitarTabLibraryStateResponse>> SetFavoriteAsync(
        GuitarTabSaveDetails details,
        bool isFavorite,
        CancellationToken cancellationToken
    )
    {
        if (!TryNormalizeUrl(details.TabUrl, out var tabUrl))
        {
            return Result.Failure<GuitarTabLibraryStateResponse>(GuitarTabSearchErrors.InvalidTabUrl);
        }

        var tab = await savedGuitarTabsRepository.GetByUserAndUrlAsync(
            userIdentifierProvider.UserId,
            tabUrl,
            cancellationToken
        );

        if (!isFavorite)
        {
            if (tab is null)
            {
                return Result.Success(EmptyState());
            }

            var memberships = await guitarPlaylistSongsRepository.ListByTabIdAsync(
                tab.Id,
                cancellationToken
            );
            if (memberships.Count > 0)
            {
                return Result.Success(State(tab, memberships));
            }

            if (!string.IsNullOrWhiteSpace(tab.YoutubeUrl))
            {
                tab.ClearFavorite(dateTimeProvider.UtcNow);
                savedGuitarTabsRepository.Update(tab);
                return Result.Success(State(tab, []));
            }

            savedGuitarTabsRepository.Remove(tab);
            return Result.Success(EmptyState());
        }

        var saved = await UpsertAsync(details with { TabUrl = tabUrl }, isFavorite: true, cancellationToken);
        if (saved.IsFailure)
        {
            return Result.Failure<GuitarTabLibraryStateResponse>(saved.Error);
        }

        var playlistIds = await guitarPlaylistSongsRepository.ListByTabIdAsync(
            saved.Value.Id,
            cancellationToken
        );
        return Result.Success(State(saved.Value, playlistIds));
    }

    public async Task<Result<GuitarTabLibraryStateResponse>> SetPlaylistsAsync(
        GuitarTabSaveDetails details,
        IReadOnlyList<GuitarPlaylistId> playlistIds,
        CancellationToken cancellationToken
    )
    {
        if (!TryNormalizeUrl(details.TabUrl, out var tabUrl))
        {
            return Result.Failure<GuitarTabLibraryStateResponse>(GuitarTabSearchErrors.InvalidTabUrl);
        }

        var distinctPlaylistIds = playlistIds.Distinct().ToArray();
        foreach (var playlistId in distinctPlaylistIds)
        {
            var playlist = await guitarPlaylistsRepository.GetByIdAndUserIdAsync(
                playlistId,
                userIdentifierProvider.UserId,
                cancellationToken
            );
            if (playlist is null)
            {
                return Result.Failure<GuitarTabLibraryStateResponse>(
                    GuitarPlaylistErrors.NotFound(playlistId.Value)
                );
            }
        }

        var normalized = details with { TabUrl = tabUrl };
        var existing = await savedGuitarTabsRepository.GetByUserAndUrlAsync(
            userIdentifierProvider.UserId,
            tabUrl,
            cancellationToken
        );

        if (distinctPlaylistIds.Length == 0)
        {
            if (existing is null)
            {
                return Result.Success(EmptyState());
            }

            if (string.IsNullOrWhiteSpace(existing.YoutubeUrl))
            {
                savedGuitarTabsRepository.Remove(existing);
                return Result.Success(EmptyState());
            }

            var existingMemberships = await guitarPlaylistSongsRepository.ListByTabIdAsync(
                existing.Id,
                cancellationToken
            );
            foreach (var membership in existingMemberships)
            {
                guitarPlaylistSongsRepository.Remove(membership);
            }

            existing.ClearFavorite(dateTimeProvider.UtcNow);
            savedGuitarTabsRepository.Update(existing);
            return Result.Success(State(existing, []));
        }

        var saved = await UpsertAsync(normalized, isFavorite: true, cancellationToken);
        if (saved.IsFailure)
        {
            return Result.Failure<GuitarTabLibraryStateResponse>(saved.Error);
        }

        var memberships = await guitarPlaylistSongsRepository.ListByTabIdAsync(
            saved.Value.Id,
            cancellationToken
        );
        var desired = distinctPlaylistIds.ToHashSet();

        foreach (var membership in memberships.Where(membership => !desired.Contains(membership.GuitarPlaylistId)))
        {
            guitarPlaylistSongsRepository.Remove(membership);
        }

        foreach (var playlistId in desired.Where(id => memberships.All(membership => membership.GuitarPlaylistId != id)))
        {
            var membership = GuitarPlaylistSong.Create(
                GuitarPlaylistSongId.NewId(),
                playlistId,
                saved.Value.Id,
                dateTimeProvider.UtcNow
            );
            if (membership.IsFailure)
            {
                return Result.Failure<GuitarTabLibraryStateResponse>(membership.Error);
            }

            await guitarPlaylistSongsRepository.AddAsync(membership.Value, cancellationToken);
        }

        return Result.Success(
            new GuitarTabLibraryStateResponse(
                true,
                desired.Select(id => id.Value).ToArray(),
                saved.Value.YoutubeUrl
            )
        );
    }

    public async Task<Result> RemoveFromPlaylistAsync(
        GuitarPlaylistId playlistId,
        SavedGuitarTabId savedGuitarTabId,
        CancellationToken cancellationToken
    )
    {
        var playlist = await guitarPlaylistsRepository.GetByIdAndUserIdAsync(
            playlistId,
            userIdentifierProvider.UserId,
            cancellationToken
        );
        if (playlist is null)
        {
            return Result.Failure(GuitarPlaylistErrors.NotFound(playlistId.Value));
        }

        var tab = await savedGuitarTabsRepository.GetByIdAsync(savedGuitarTabId, cancellationToken);
        if (tab is null || tab.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(SavedGuitarTabErrors.NotFound(savedGuitarTabId.Value));
        }

        var memberships = await guitarPlaylistSongsRepository.ListByTabIdAsync(
            tab.Id,
            cancellationToken
        );
        var membership = memberships.FirstOrDefault(song => song.GuitarPlaylistId == playlistId);
        if (membership is null)
        {
            return Result.Success();
        }

        if (memberships.All(song => song.GuitarPlaylistId == playlistId))
        {
            if (string.IsNullOrWhiteSpace(tab.YoutubeUrl))
            {
                savedGuitarTabsRepository.Remove(tab);
            }
            else
            {
                guitarPlaylistSongsRepository.Remove(membership);
                tab.ClearFavorite(dateTimeProvider.UtcNow);
                savedGuitarTabsRepository.Update(tab);
            }
        }
        else
        {
            guitarPlaylistSongsRepository.Remove(membership);
        }

        return Result.Success();
    }

    public async Task RemoveSongsOnlyInPlaylistAsync(
        GuitarPlaylistId playlistId,
        CancellationToken cancellationToken
    )
    {
        var memberships = await guitarPlaylistSongsRepository.ListByPlaylistIdAsync(
            playlistId,
            cancellationToken
        );
        var tabIds = memberships.Select(song => song.SavedGuitarTabId).Distinct().ToArray();
        if (tabIds.Length == 0)
        {
            return;
        }

        var related = await guitarPlaylistSongsRepository.ListByTabIdsAsync(tabIds, cancellationToken);
        foreach (var tabId in tabIds)
        {
            var remainsInAnotherPlaylist = related.Any(song =>
                song.SavedGuitarTabId == tabId && song.GuitarPlaylistId != playlistId
            );
            if (remainsInAnotherPlaylist)
            {
                continue;
            }

            var tab = await savedGuitarTabsRepository.GetByIdAsync(tabId, cancellationToken);
            if (tab is null)
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(tab.YoutubeUrl))
            {
                savedGuitarTabsRepository.Remove(tab);
                continue;
            }

            tab.ClearFavorite(dateTimeProvider.UtcNow);
            savedGuitarTabsRepository.Update(tab);
        }
    }

    private async Task<Result<SavedGuitarTab>> UpsertAsync(
        GuitarTabSaveDetails details,
        bool isFavorite,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var now = dateTimeProvider.UtcNow;
        var existing = await savedGuitarTabsRepository.GetByUserAndUrlAsync(
            userId,
            details.TabUrl,
            cancellationToken
        );

        if (existing is null)
        {
            var created = SavedGuitarTab.Create(
                SavedGuitarTabId.NewId(),
                userId,
                details.TabUrl,
                details.SongName,
                details.ArtistName,
                details.Type,
                isFavorite,
                now
            );
            if (created.IsFailure)
            {
                return created;
            }

            await savedGuitarTabsRepository.AddAsync(created.Value, cancellationToken);
            return created;
        }

        existing.UpdateDetails(details.SongName, details.ArtistName, details.Type, now);
        if (isFavorite)
        {
            existing.MarkFavorite(now);
        }

        savedGuitarTabsRepository.Update(existing);
        return Result.Success(existing);
    }

    public async Task<Result<GuitarTabLibraryStateResponse>> SetYoutubeUrlAsync(
        GuitarTabSaveDetails details,
        string? youtubeUrl,
        CancellationToken cancellationToken
    )
    {
        if (!TryNormalizeUrl(details.TabUrl, out var tabUrl))
        {
            return Result.Failure<GuitarTabLibraryStateResponse>(GuitarTabSearchErrors.InvalidTabUrl);
        }

        var hasVideo = YoutubeVideoUrl.TryNormalize(youtubeUrl, out var normalized);
        if (!string.IsNullOrWhiteSpace(youtubeUrl) && !hasVideo)
        {
            return Result.Failure<GuitarTabLibraryStateResponse>(SavedGuitarTabErrors.InvalidYoutubeUrl);
        }

        var existing = await savedGuitarTabsRepository.GetByUserAndUrlAsync(
            userIdentifierProvider.UserId,
            tabUrl,
            cancellationToken
        );

        if (!hasVideo)
        {
            if (existing is null)
            {
                return Result.Success(EmptyState());
            }

            var cleared = existing.SetYoutubeUrl(null, dateTimeProvider.UtcNow);
            if (cleared.IsFailure)
            {
                return Result.Failure<GuitarTabLibraryStateResponse>(cleared.Error);
            }

            var memberships = await guitarPlaylistSongsRepository.ListByTabIdAsync(
                existing.Id,
                cancellationToken
            );
            if (!existing.IsFavorite && memberships.Count == 0)
            {
                savedGuitarTabsRepository.Remove(existing);
                return Result.Success(EmptyState());
            }

            savedGuitarTabsRepository.Update(existing);
            return Result.Success(State(existing, memberships));
        }

        var saved = await UpsertAsync(details with { TabUrl = tabUrl }, isFavorite: false, cancellationToken);
        if (saved.IsFailure)
        {
            return Result.Failure<GuitarTabLibraryStateResponse>(saved.Error);
        }

        var updated = saved.Value.SetYoutubeUrl(normalized, dateTimeProvider.UtcNow);
        if (updated.IsFailure)
        {
            return Result.Failure<GuitarTabLibraryStateResponse>(updated.Error);
        }

        if (existing is not null)
        {
            savedGuitarTabsRepository.Update(saved.Value);
        }
        var playlistIds = await guitarPlaylistSongsRepository.ListByTabIdAsync(
            saved.Value.Id,
            cancellationToken
        );
        return Result.Success(State(saved.Value, playlistIds));
    }

    private static bool TryNormalizeUrl(string url, out string normalized)
    {
        if (!UltimateGuitarTabUrl.TryCreate(url, out var tabUrl))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = tabUrl.AbsoluteUri;
        return true;
    }

    private static GuitarTabLibraryStateResponse EmptyState() => new(false, [], null);

    private static GuitarTabLibraryStateResponse State(
        SavedGuitarTab tab,
        IReadOnlyList<GuitarPlaylistSong> memberships
    ) =>
        new(
            tab.IsFavorite,
            memberships.Select(membership => membership.GuitarPlaylistId.Value).ToArray(),
            tab.YoutubeUrl
        );
}
