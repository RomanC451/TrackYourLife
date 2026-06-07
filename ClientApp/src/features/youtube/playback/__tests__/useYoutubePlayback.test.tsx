import { renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

const {
  mockNavigate,
  mockOpenYoutubePlayer,
  mockCloseYoutubePlayer,
  mockMinimizeYoutubePlayer,
  mockUseMutationState,
  mockPathname,
} = vi.hoisted(() => ({
  mockNavigate: vi.fn(),
  mockOpenYoutubePlayer: vi.fn(),
  mockCloseYoutubePlayer: vi.fn(),
  mockMinimizeYoutubePlayer: vi.fn(),
  mockUseMutationState: vi.fn(),
  mockPathname: { current: "/youtube/videos" },
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
  useLocation: () => ({ pathname: mockPathname.current }),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useMutationState: (...args: unknown[]) => mockUseMutationState(...args),
  };
});

vi.mock("../../contexts/YoutubePlayerHostContext", () => ({
  useYoutubePlayerHost: () => ({
    openYoutubePlayer: mockOpenYoutubePlayer,
    closeYoutubePlayer: mockCloseYoutubePlayer,
    minimizeYoutubePlayer: mockMinimizeYoutubePlayer,
    playerState: { videoId: "video-1" },
  }),
}));

import { useYoutubePlayback } from "../useYoutubePlayback";

describe("useYoutubePlayback", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockPathname.current = "/youtube/videos";
    mockUseMutationState.mockReturnValue([]);
  });

  it("opens the player and syncs the watch route", () => {
    const { result } = renderHook(() => useYoutubePlayback());

    result.current.openYoutubeVideo("video-2");

    expect(mockOpenYoutubePlayer).toHaveBeenCalledWith({ videoId: "video-2" });
    expect(mockNavigate).toHaveBeenCalledWith({
      to: "/youtube/videos/watch/$videoId",
      params: { videoId: "video-2" },
    });
  });

  it("can open without syncing the route", () => {
    const { result } = renderHook(() => useYoutubePlayback());

    result.current.openYoutubeVideo("video-2", { syncRoute: false });

    expect(mockOpenYoutubePlayer).toHaveBeenCalledWith({ videoId: "video-2" });
    expect(mockNavigate).not.toHaveBeenCalled();
  });

  it("does not sync the route from unsupported pages", () => {
    mockPathname.current = "/youtube/channels";

    const { result } = renderHook(() => useYoutubePlayback());

    result.current.openYoutubeVideo("video-2");

    expect(mockOpenYoutubePlayer).toHaveBeenCalledWith({ videoId: "video-2" });
    expect(mockNavigate).not.toHaveBeenCalled();
  });

  it("does not navigate when already on the watch route", () => {
    mockPathname.current = "/youtube/videos/watch/video-2";

    const { result } = renderHook(() => useYoutubePlayback());

    result.current.openYoutubeVideo("video-2");

    expect(mockOpenYoutubePlayer).toHaveBeenCalledWith({ videoId: "video-2" });
    expect(mockNavigate).not.toHaveBeenCalled();
  });

  it("exposes player controls and pending state", () => {
    mockUseMutationState.mockReturnValue([{ status: "pending" }]);

    const { result } = renderHook(() => useYoutubePlayback());

    expect(result.current.currentVideoId).toBe("video-1");
    expect(result.current.isPlayPending).toBe(true);
    expect(result.current.closeYoutubeVideo).toBe(mockCloseYoutubePlayer);
    expect(result.current.minimizeYoutubeVideo).toBe(mockMinimizeYoutubePlayer);
  });
});
