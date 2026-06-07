import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";
import { queryClient } from "@/queryClient";

const { mockPlayVideo, mockNavigate, mockToastError } = vi.hoisted(() => ({
  mockPlayVideo: vi.fn(),
  mockNavigate: vi.fn(),
  mockToastError: vi.fn(),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockVideosApi {
    playVideo = mockPlayVideo;
  }
  return { ...actual, VideosApi: MockVideosApi };
});

vi.mock("sonner", () => ({
  toast: {
    error: mockToastError,
  },
}));

import usePlayVideoMutation from "../usePlayVideoMutation";
import { youtubeQueryKeys } from "../../queries/youtubeQueries";

describe("usePlayVideoMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    queryClient.clear();
    mockPlayVideo.mockResolvedValue({ data: { videoId: "video-1" } });
  });

  it("plays a video and caches details", async () => {
    const { result } = renderHook(() => usePlayVideoMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync("video-1");
    });

    expect(mockPlayVideo).toHaveBeenCalledWith("video-1");
    expect(queryClient.getQueryData(youtubeQueryKeys.videoDetails("video-1"))).toEqual(
      { videoId: "video-1" },
    );
  });

  it("does not cache details when the API returns no payload", async () => {
    mockPlayVideo.mockResolvedValue({ data: undefined });

    const { result } = renderHook(() => usePlayVideoMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync("video-1");
    });

    expect(
      queryClient.getQueryData(youtubeQueryKeys.videoDetails("video-1")),
    ).toBeUndefined();
  });

  it("shows a settings action when the entertainment limit is reached", async () => {
    mockPlayVideo.mockRejectedValue({
      response: {
        status: 403,
        data: { type: "Youtube.EntertainmentLimitReached" },
      },
    });

    const { result } = renderHook(() => usePlayVideoMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync("video-1").catch(() => undefined);
    });

    expect(mockToastError).toHaveBeenCalledWith(
      "Limit reached.",
      expect.objectContaining({
        action: expect.objectContaining({ label: "Go to settings" }),
      }),
    );

    const toastArgs = mockToastError.mock.calls[0]?.[1] as {
      action?: { onClick?: () => void };
    };
    toastArgs.action?.onClick?.();
    expect(mockNavigate).toHaveBeenCalledWith({ to: "/youtube/settings" });
  });
});
