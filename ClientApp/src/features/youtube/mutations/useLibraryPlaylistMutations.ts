import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { LibraryApi } from "@/services/openapi";

import { youtubeQueryKeys } from "../queries/youtubeQueries";

const libraryApi = new LibraryApi();

export function useCreatePlaylistMutation() {
  return useCustomMutation({
    mutationFn: (name: string) =>
      libraryApi.createPlaylist({ name }).then((res) => res.data),
    meta: {
      onSuccessToast: { message: "Playlist created", type: "success" },
      invalidateQueries: [youtubeQueryKeys.libraryPlaylists()],
    },
  });
}

export function useUpdatePlaylistMutation() {
  return useCustomMutation({
    mutationFn: ({ playlistId, name }: { playlistId: string; name: string }) =>
      libraryApi.updatePlaylist(playlistId, { name }),
    meta: {
      onSuccessToast: { message: "Playlist updated", type: "success" },
    },
    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: youtubeQueryKeys.libraryPlaylists(),
      });
      await queryClient.invalidateQueries({
        queryKey: youtubeQueryKeys.libraryPlaylist(variables.playlistId),
      });
    },
  });
}

export function useDeletePlaylistMutation() {
  return useCustomMutation({
    mutationFn: (playlistId: string) => libraryApi.deletePlaylist(playlistId),
    meta: {
      onSuccessToast: { message: "Playlist deleted", type: "success" },
      invalidateQueries: [youtubeQueryKeys.libraryPlaylists()],
    },
  });
}

export function useAddVideoToPlaylistMutation() {
  return useCustomMutation({
    mutationFn: ({
      playlistId,
      videoId,
    }: {
      playlistId: string;
      videoId: string;
    }) => libraryApi.addVideoToPlaylist(playlistId, { videoId }),
    meta: {
      onSuccessToast: { message: "Added to playlist", type: "success" },
    },
    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: youtubeQueryKeys.libraryPlaylists(),
      });
      await queryClient.invalidateQueries({
        queryKey: youtubeQueryKeys.libraryPlaylist(variables.playlistId),
      });
    },
  });
}

export function useRemoveVideoFromPlaylistMutation() {
  return useCustomMutation({
    mutationFn: ({
      playlistId,
      videoId,
    }: {
      playlistId: string;
      videoId: string;
    }) => libraryApi.removeVideoFromPlaylist(playlistId, videoId),
    meta: {
      onSuccessToast: { message: "Removed from playlist", type: "success" },
    },
    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({
        queryKey: youtubeQueryKeys.libraryPlaylist(variables.playlistId),
      });
      await queryClient.invalidateQueries({
        queryKey: youtubeQueryKeys.libraryPlaylists(),
      });
    },
  });
}
