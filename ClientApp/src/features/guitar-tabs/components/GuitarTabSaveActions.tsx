import { Heart, ListMusic } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import { useCustomQuery } from "@/hooks/useCustomQuery";

import type { GuitarPlaylist, GuitarTabContent } from "../api/guitarTabsApi";
import {
  useSetGuitarTabFavoriteMutation,
  useSetGuitarTabPlaylistsMutation,
} from "../mutations/useSavedGuitarTabMutations";
import { guitarTabsQueryOptions } from "../queries/guitarTabsQueries";

export function GuitarTabSaveControls({
  isFavorite,
  playlists,
  selectedPlaylistIds,
  pending,
  playlistsReady,
  onFavoriteChange,
  onPlaylistsChange,
}: {
  isFavorite: boolean;
  playlists: GuitarPlaylist[];
  selectedPlaylistIds: string[];
  pending: boolean;
  playlistsReady: boolean;
  onFavoriteChange: (isFavorite: boolean) => void;
  onPlaylistsChange: (playlistIds: string[]) => void;
}) {
  const keptByPlaylist = isFavorite && selectedPlaylistIds.length > 0;

  return (
    <div className="flex items-center gap-2">
      <Button
        type="button"
        variant="outline"
        size="icon"
        aria-label={isFavorite ? "Remove from favorites" : "Add to favorites"}
        aria-pressed={isFavorite}
        disabled={pending}
        title={
          keptByPlaylist
            ? "This song stays in Favorites while it is in a playlist"
            : undefined
        }
        onClick={() => {
          if (keptByPlaylist) {
            return;
          }

          onFavoriteChange(!isFavorite);
        }}
      >
        <Heart className={isFavorite ? "fill-red-500 text-red-500" : undefined} />
      </Button>
      <Popover>
        <PopoverTrigger asChild>
          <Button type="button" variant="outline" disabled={pending}>
            <ListMusic />
            Playlists
          </Button>
        </PopoverTrigger>
        <PopoverContent align="end" className="flex flex-col gap-3">
          {!playlistsReady ? (
            <p className="text-sm text-muted-foreground">Loading playlists...</p>
          ) : playlists.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              Create a playlist from the sidebar to add this song.
            </p>
          ) : (
            playlists.map((playlist) => {
              const checked = selectedPlaylistIds.includes(playlist.id);

              return (
                <label
                  key={playlist.id}
                  className="flex items-center gap-2 text-sm"
                >
                  <Checkbox
                    checked={checked}
                    onCheckedChange={(value) => {
                      const next = value
                        ? [...selectedPlaylistIds, playlist.id]
                        : selectedPlaylistIds.filter((id) => id !== playlist.id);
                      onPlaylistsChange(next);
                    }}
                  />
                  {playlist.name}
                </label>
              );
            })
          )}
        </PopoverContent>
      </Popover>
    </div>
  );
}

function GuitarTabSaveActions({ tab }: { tab: GuitarTabContent }) {
  const { query: savedQuery } = useCustomQuery(
    guitarTabsQueryOptions.savedTab(tab.tabUrl),
  );
  const { query: playlistsQuery } = useCustomQuery(
    guitarTabsQueryOptions.playlists(),
  );
  const favoriteMutation = useSetGuitarTabFavoriteMutation();
  const playlistsMutation = useSetGuitarTabPlaylistsMutation();
  const saved = savedQuery.data ?? { isFavorite: false, playlistIds: [] };
  const request = {
    tabUrl: tab.tabUrl,
    songName: tab.songName,
    artistName: tab.artistName,
    type: tab.type,
  };

  return (
    <GuitarTabSaveControls
      isFavorite={saved.isFavorite}
      playlists={playlistsQuery.data ?? []}
      selectedPlaylistIds={saved.playlistIds}
      pending={
        savedQuery.isPending ||
        favoriteMutation.isPending ||
        playlistsMutation.isPending
      }
      playlistsReady={playlistsQuery.isSuccess}
      onFavoriteChange={(isFavorite) => {
        favoriteMutation.mutate({ ...request, isFavorite });
      }}
      onPlaylistsChange={(playlistIds) => {
        playlistsMutation.mutate({ ...request, playlistIds });
      }}
    />
  );
}

export default GuitarTabSaveActions;
