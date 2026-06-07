import { fireEvent, render, screen } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { watchHistoryEntry } from "@/features/youtube/__tests__/fixtures";

const { mockOpenYoutubeVideo } = vi.hoisted(() => ({
  mockOpenYoutubeVideo: vi.fn(),
}));

vi.mock("@/features/youtube/playback/useYoutubePlayback", () => ({
  useYoutubePlayback: () => ({
    openYoutubeVideo: mockOpenYoutubeVideo,
  }),
}));

vi.mock("@tanstack/react-router", () => ({
  Link: ({ children, to }: { children: React.ReactNode; to: string }) => (
    <a href={to}>{children}</a>
  ),
}));

import WatchHistoryEntryCard from "../WatchHistoryEntryCard";

describe("WatchHistoryEntryCard", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("renders a playable history entry", () => {
    render(<WatchHistoryEntryCard entry={watchHistoryEntry()} />);

    fireEvent.click(screen.getAllByRole("button", { name: /Video video-1/i })[1]!);

    expect(mockOpenYoutubeVideo).toHaveBeenCalledWith("video-1");
    expect(screen.getByText(/Watched Today/i)).toBeInTheDocument();
    expect(screen.getByText("1.5K")).toBeInTheDocument();
  });

  it("handles missing video details", () => {
    render(
      <WatchHistoryEntryCard
        entry={watchHistoryEntry({ video: undefined, videoId: "missing-id" })}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /Open video/i }));

    expect(screen.getByText("missing-id")).toBeInTheDocument();
    expect(mockOpenYoutubeVideo).toHaveBeenCalledWith("missing-id");
  });
});
