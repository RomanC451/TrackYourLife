import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";

import {
  createGuitarPlaylist,
  deleteGuitarPlaylist,
  updateGuitarPlaylist,
} from "../api/guitarTabsApi";
import { guitarTabsQueryKeys } from "../queries/guitarTabsQueries";

export function useCreateGuitarPlaylistMutation() {
  return useCustomMutation({
    mutationFn: (name: string) => createGuitarPlaylist(name),
    meta: {
      onSuccessToast: { message: "Playlist created", type: "success" },
      invalidateQueries: [guitarTabsQueryKeys.playlists()],
    },
  });
}

export function useUpdateGuitarPlaylistMutation() {
  return useCustomMutation({
    mutationFn: ({ playlistId, name }: { playlistId: string; name: string }) =>
      updateGuitarPlaylist(playlistId, name),
    meta: {
      onSuccessToast: { message: "Playlist updated", type: "success" },
    },
    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: guitarTabsQueryKeys.playlists(),
      });
      await queryClient.invalidateQueries({
        queryKey: guitarTabsQueryKeys.playlist(variables.playlistId),
      });
    },
  });
}

export function useDeleteGuitarPlaylistMutation() {
  return useCustomMutation({
    mutationFn: (playlistId: string) => deleteGuitarPlaylist(playlistId),
    meta: {
      onSuccessToast: { message: "Playlist deleted", type: "success" },
      invalidateQueries: [
        guitarTabsQueryKeys.playlists(),
        guitarTabsQueryKeys.favorites(),
        guitarTabsQueryKeys.playlistSongsAll(),
        guitarTabsQueryKeys.savedTabs(),
      ],
    },
  });
}
