import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { fireEvent, render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import type { ReactNode } from "react";

import { youtubeQueryKeys } from "@/features/youtube/queries/youtubeQueries";

const {
  mockNavigate,
  mockMutate,
  mockDisableBodyScroll,
  mockEnableBodyScroll,
  mockPlayMutation,
} = vi.hoisted(() => ({
  mockNavigate: vi.fn(),
  mockMutate: vi.fn(),
  mockDisableBodyScroll: vi.fn(),
  mockEnableBodyScroll: vi.fn(),
  mockPlayMutation: {
    mutate: vi.fn(),
    isPending: false,
    isError: false,
    error: null,
  },
}));

function setPlayMutationState(
  state: Partial<typeof mockPlayMutation>,
) {
  Object.assign(mockPlayMutation, {
    mutate: mockMutate,
    isPending: false,
    isError: false,
    error: null,
    ...state,
  });
}

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("@/lib/bodyScroll", () => ({
  disableBodyScroll: mockDisableBodyScroll,
  enableBodyScroll: mockEnableBodyScroll,
}));

vi.mock("../../../playback/usePlayVideoMutation", () => ({
  default: () => mockPlayMutation,
}));

vi.mock("../../channels/SubscribeChannelDropdown", () => ({
  default: () => <div data-testid="subscribe-dropdown" />,
}));

vi.mock("../../library/AddToPlaylistDropdown", () => ({
  default: () => <div data-testid="playlist-dropdown" />,
}));

import VideoPlayerDialog from "../VideoPlayerDialog";

const videoDetails = {
  videoId: "video-1",
  title: "Morning workout",
  channelId: "ch-1",
  channelName: "Fitness Channel",
  embedUrl: "https://example.com/embed",
  viewCount: 1200,
  likeCount: 80,
  publishedAt: "2026-06-01T00:00:00Z",
  description: "A great workout",
};

const testQueryClient = new QueryClient({
  defaultOptions: {
    queries: { retry: false, gcTime: 0 },
    mutations: { retry: false },
  },
});

function TestWrapper({ children }: { children: ReactNode }) {
  return (
    <QueryClientProvider client={testQueryClient}>{children}</QueryClientProvider>
  );
}

describe("VideoPlayerDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    setPlayMutationState({});
    testQueryClient.clear();
    testQueryClient.setQueryData(
      youtubeQueryKeys.videoDetails("video-1"),
      videoDetails,
    );
    class ResizeObserverMock {
      observe() {}
      unobserve() {}
      disconnect() {}
    }
    vi.stubGlobal("ResizeObserver", ResizeObserverMock);
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders video details after playback", () => {
    render(<VideoPlayerDialog videoId="video-1" onClose={vi.fn()} />, {
      wrapper: TestWrapper,
    });

    expect(mockMutate).toHaveBeenCalledWith("video-1", expect.any(Object));
    expect(screen.getByText("Morning workout")).toBeInTheDocument();
    expect(screen.getByText("Fitness Channel")).toBeInTheDocument();
    expect(screen.getByTestId("subscribe-dropdown")).toBeInTheDocument();
  });

  it("closes via custom handler", () => {
    const onClose = vi.fn();

    render(<VideoPlayerDialog videoId="video-1" onClose={onClose} />, {
      wrapper: TestWrapper,
    });

    fireEvent.click(screen.getByRole("button", { name: /Close/i }));
    expect(onClose).toHaveBeenCalled();
  });

  it("shows loading UI while playback is pending", () => {
    setPlayMutationState({ isPending: true });

    const { container } = render(<VideoPlayerDialog videoId="video-1" />, {
      wrapper: TestWrapper,
    });

    expect(container.querySelector(".animate-pulse")).toBeInTheDocument();
    expect(screen.queryByText("Morning workout")).not.toBeInTheDocument();
  });

  it("navigates back to videos when closed without a custom handler", () => {
    render(<VideoPlayerDialog videoId="video-1" />, { wrapper: TestWrapper });

    fireEvent.click(screen.getByRole("button", { name: /Close/i }));

    expect(mockNavigate).toHaveBeenCalledWith({ to: "/youtube/videos" });
  });

  it("locks body scroll while visible", () => {
    const { unmount } = render(
      <VideoPlayerDialog videoId="video-1" isVisible={false} />,
      { wrapper: TestWrapper },
    );

    expect(mockEnableBodyScroll).toHaveBeenCalled();
    unmount();
    expect(mockEnableBodyScroll).toHaveBeenCalledTimes(2);
  });

  it("formats stats and shows fallback description", () => {
    testQueryClient.setQueryData(youtubeQueryKeys.videoDetails("video-1"), {
      ...videoDetails,
      viewCount: 1_200_000,
      likeCount: 2500,
      description: "",
    });

    render(<VideoPlayerDialog videoId="video-1" onClose={vi.fn()} />, {
      wrapper: TestWrapper,
    });

    expect(screen.getByText("1.2M views")).toBeInTheDocument();
    expect(screen.getByText("2.5K likes")).toBeInTheDocument();
    expect(screen.getByText("No description available")).toBeInTheDocument();
  });

  it("reveals the iframe after repeated load events", () => {
    vi.useFakeTimers();

    render(<VideoPlayerDialog videoId="video-1" onClose={vi.fn()} />, {
      wrapper: TestWrapper,
    });

    const iframe = screen.getByTitle("Morning workout");
    fireEvent.load(iframe);
    fireEvent.load(iframe);

    expect(iframe.className).toContain("opacity-100");
  });
});
