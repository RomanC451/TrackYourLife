import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeChannel } from "@/features/youtube/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockSetChannelFavorite, mockToastError } = vi.hoisted(() => ({
  mockSetChannelFavorite: vi.fn(),
  mockToastError: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockChannelsApi {
    setChannelFavorite = mockSetChannelFavorite;
  }
  return { ...actual, ChannelsApi: MockChannelsApi };
});

vi.mock("sonner", () => ({
  toast: {
    error: mockToastError,
  },
}));

import useSetChannelFavoriteMutation from "../useSetChannelFavoriteMutation";
import { youtubeQueryKeys } from "../../queries/youtubeQueries";

describe("useSetChannelFavoriteMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    queryClient.clear();
    mockSetChannelFavorite.mockResolvedValue({ data: undefined });
  });

  it("optimistically updates favorite state", async () => {
    queryClient.setQueryData(youtubeQueryKeys.channels("all"), [
      youtubeChannel("ch-1", { isFavorite: false }),
    ]);

    const { result } = renderHook(() => useSetChannelFavoriteMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        youtubeChannelId: "ch-1",
        isFavorite: true,
      });
    });

    expect(mockSetChannelFavorite).toHaveBeenCalledWith("ch-1", { isFavorite: true });
    expect(
      queryClient.getQueryData<ReturnType<typeof youtubeChannel>[]>(
        youtubeQueryKeys.channels("all"),
      )?.[0]?.isFavorite,
    ).toBe(true);
  });

  it("removes unfavorited channels from the favorites list optimistically", async () => {
    queryClient.setQueryData(youtubeQueryKeys.channels("favorites"), [
      youtubeChannel("ch-1", { isFavorite: true }),
      youtubeChannel("ch-2", { isFavorite: true }),
    ]);

    const { result } = renderHook(() => useSetChannelFavoriteMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        youtubeChannelId: "ch-1",
        isFavorite: false,
      });
    });

    expect(
      queryClient.getQueryData<ReturnType<typeof youtubeChannel>[]>(
        youtubeQueryKeys.channels("favorites"),
      ),
    ).toEqual([youtubeChannel("ch-2", { isFavorite: true })]);
  });

  it("rolls back favorite changes when the API fails", async () => {
    const original = [youtubeChannel("ch-1", { isFavorite: false })];
    queryClient.setQueryData(youtubeQueryKeys.channels("all"), original);
    mockSetChannelFavorite.mockRejectedValue({
      response: { data: { detail: "Favorite failed" } },
    });

    const { result } = renderHook(() => useSetChannelFavoriteMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current
        .mutateAsync({ youtubeChannelId: "ch-1", isFavorite: true })
        .catch(() => undefined);
    });

    expect(queryClient.getQueryData(youtubeQueryKeys.channels("all"))).toEqual(
      original,
    );
    expect(mockToastError).toHaveBeenCalledWith("Favorite failed");
  });
});
