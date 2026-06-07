import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeVideo } from "@/features/youtube/__tests__/fixtures";

const { mockOpenYoutubeVideo } = vi.hoisted(() => ({
  mockOpenYoutubeVideo: vi.fn(),
}));

vi.mock("@/features/youtube/playback/useYoutubePlayback", () => ({
  useYoutubePlayback: () => ({
    openYoutubeVideo: mockOpenYoutubeVideo,
  }),
}));

vi.mock("../../library/AddToPlaylistDropdown", () => ({
  default: () => <button type="button">Add to playlist</button>,
}));

vi.mock("@tanstack/react-router", () => ({
  Link: ({ children, to }: { children: React.ReactNode; to: string }) => (
    <a href={to}>{children}</a>
  ),
}));

import VideoCard from "../VideoCard";
import { suppressYoutubeCardClick } from "../../../youtubeClickGuard";

describe("VideoCard", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("opens a video on click", () => {
    render(<VideoCard video={youtubeVideo("v-1")} />);

    fireEvent.click(screen.getAllByRole("button", { name: /Video v-1/i })[1]!);

    expect(mockOpenYoutubeVideo).toHaveBeenCalledWith("v-1");
  });

  it("ignores clicks while suppression is active", () => {
    suppressYoutubeCardClick(500);

    render(<VideoCard video={youtubeVideo("v-1")} />);
    fireEvent.click(screen.getAllByRole("button", { name: /Video v-1/i })[1]!);

    expect(mockOpenYoutubeVideo).not.toHaveBeenCalled();
  });

  it("renders watched and featured layouts", () => {
    render(
      <VideoCard
        video={youtubeVideo("v-1", { isWatched: true, viewCount: 2500000 })}
        layout="featured"
      />,
    );

    expect(screen.getByText("Watched")).toBeInTheDocument();
    expect(screen.getByText("2.5M")).toBeInTheDocument();
  });
});
