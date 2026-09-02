import { renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const { mockOpenYoutubeVideo, playbackState } = vi.hoisted(() => ({
  mockOpenYoutubeVideo: vi.fn(),
  playbackState: { currentVideoId: undefined as string | undefined },
}));

vi.mock("../useYoutubePlayback", () => ({
  useYoutubePlayback: () => ({
    openYoutubeVideo: mockOpenYoutubeVideo,
    currentVideoId: playbackState.currentVideoId,
  }),
}));

import { useSyncWatchRoutePlayer } from "../useSyncWatchRoutePlayer";

describe("useSyncWatchRoutePlayer", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    playbackState.currentVideoId = undefined;
  });

  it("opens the player when landing on a watch route with no active video", () => {
    renderHook(() => useSyncWatchRoutePlayer("video-1"));

    expect(mockOpenYoutubeVideo).toHaveBeenCalledWith("video-1", {
      syncRoute: false,
    });
  });

  it("does not reopen the player when this video is already playing", () => {
    playbackState.currentVideoId = "video-1";

    const { rerender } = renderHook(() => useSyncWatchRoutePlayer("video-1"));

    expect(mockOpenYoutubeVideo).not.toHaveBeenCalled();

    rerender();

    expect(mockOpenYoutubeVideo).not.toHaveBeenCalled();
  });

  it("opens a different video from the watch route", () => {
    playbackState.currentVideoId = "video-1";

    renderHook(() => useSyncWatchRoutePlayer("video-2"));

    expect(mockOpenYoutubeVideo).toHaveBeenCalledWith("video-2", {
      syncRoute: false,
    });
  });
});
