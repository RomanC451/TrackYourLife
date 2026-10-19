import { toast } from "sonner";

import { useCustomMutation } from "@/hooks/useCustomMutation";

import {
  removeGuitarTabFromPlaylist,
  setSavedGuitarTabFavorite,
  setSavedGuitarTabPlaylists,
  setSavedGuitarTabYoutube,
  type GuitarTabSaveRequest,
} from "../api/guitarTabsApi";
import { guitarTabsQueryKeys } from "../queries/guitarTabsQueries";

const libraryInvalidation = [
  guitarTabsQueryKeys.savedTabs(),
  guitarTabsQueryKeys.favorites(),
  guitarTabsQueryKeys.playlistSongsAll(),
];

export function useSetGuitarTabFavoriteMutation() {
  return useCustomMutation({
    mutationFn: (request: GuitarTabSaveRequest & { isFavorite: boolean }) =>
      setSavedGuitarTabFavorite(request),
    meta: {
      invalidateQueries: libraryInvalidation,
    },
    onSuccess: (_, variables) => {
      toast.success(
        variables.isFavorite ? "Added to favorites" : "Removed from favorites",
      );
    },
  });
}

export function useSetGuitarTabPlaylistsMutation() {
  return useCustomMutation({
    mutationFn: (request: GuitarTabSaveRequest & { playlistIds: string[] }) =>
      setSavedGuitarTabPlaylists(request),
    meta: {
      onSuccessToast: { message: "Playlist updated", type: "success" },
      invalidateQueries: libraryInvalidation,
    },
  });
}

export function useSetGuitarTabYoutubeMutation() {
  return useCustomMutation({
    mutationFn: (request: GuitarTabSaveRequest & { youtubeUrl: string | null }) =>
      setSavedGuitarTabYoutube(request),
    meta: {
      invalidateQueries: libraryInvalidation,
    },
    onSuccess: (_, variables) => {
      toast.success(variables.youtubeUrl ? "Video saved" : "Video removed");
    },
  });
}

export function useRemoveGuitarTabFromPlaylistMutation() {
  return useCustomMutation({
    mutationFn: ({
      playlistId,
      songId,
    }: {
      playlistId: string;
      songId: string;
    }) => removeGuitarTabFromPlaylist(playlistId, songId),
    meta: {
      onSuccessToast: { message: "Removed from playlist", type: "success" },
      invalidateQueries: libraryInvalidation,
    },
  });
}
