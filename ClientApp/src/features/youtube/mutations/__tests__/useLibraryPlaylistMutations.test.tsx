import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const {
  mockCreatePlaylist,
  mockUpdatePlaylist,
  mockDeletePlaylist,
  mockAddVideoToPlaylist,
  mockRemoveVideoFromPlaylist,
} = vi.hoisted(() => ({
  mockCreatePlaylist: vi.fn(),
  mockUpdatePlaylist: vi.fn(),
  mockDeletePlaylist: vi.fn(),
  mockAddVideoToPlaylist: vi.fn(),
  mockRemoveVideoFromPlaylist: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockLibraryApi {
    createPlaylist = mockCreatePlaylist;
    updatePlaylist = mockUpdatePlaylist;
    deletePlaylist = mockDeletePlaylist;
    addVideoToPlaylist = mockAddVideoToPlaylist;
    removeVideoFromPlaylist = mockRemoveVideoFromPlaylist;
  }
  return { ...actual, LibraryApi: MockLibraryApi };
});

import {
  useAddVideoToPlaylistMutation,
  useCreatePlaylistMutation,
  useDeletePlaylistMutation,
  useRemoveVideoFromPlaylistMutation,
  useUpdatePlaylistMutation,
} from "../useLibraryPlaylistMutations";

describe("useLibraryPlaylistMutations", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockCreatePlaylist.mockResolvedValue({ data: { id: "pl-1" } });
    mockUpdatePlaylist.mockResolvedValue({ data: undefined });
    mockDeletePlaylist.mockResolvedValue({ data: undefined });
    mockAddVideoToPlaylist.mockResolvedValue({ data: undefined });
    mockRemoveVideoFromPlaylist.mockResolvedValue({ data: undefined });
  });

  it("creates playlists", async () => {
    const { result } = renderHook(() => useCreatePlaylistMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync("Workouts");
    });

    expect(mockCreatePlaylist).toHaveBeenCalledWith({ name: "Workouts" });
  });

  it("updates playlists", async () => {
    const { result } = renderHook(() => useUpdatePlaylistMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ playlistId: "pl-1", name: "Stretching" });
    });

    expect(mockUpdatePlaylist).toHaveBeenCalledWith("pl-1", { name: "Stretching" });
  });

  it("deletes playlists", async () => {
    const { result } = renderHook(() => useDeletePlaylistMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync("pl-1");
    });

    expect(mockDeletePlaylist).toHaveBeenCalledWith("pl-1");
  });

  it("adds and removes playlist videos", async () => {
    const addHook = renderHook(() => useAddVideoToPlaylistMutation(), {
      wrapper: createQueryClientWrapper(),
    });
    const removeHook = renderHook(() => useRemoveVideoFromPlaylistMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await addHook.result.current.mutateAsync({
        playlistId: "pl-1",
        videoId: "video-1",
      });
      await removeHook.result.current.mutateAsync({
        playlistId: "pl-1",
        videoId: "video-1",
      });
    });

    expect(mockAddVideoToPlaylist).toHaveBeenCalledWith("pl-1", {
      videoId: "video-1",
    });
    expect(mockRemoveVideoFromPlaylist).toHaveBeenCalledWith("pl-1", "video-1");
  });
});
