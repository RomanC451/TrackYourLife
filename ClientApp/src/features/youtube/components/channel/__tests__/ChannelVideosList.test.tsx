import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { youtubeVideo } from "@/features/youtube/__tests__/fixtures";

vi.mock("../../videosList/VideoCard", () => ({
  default: ({ video }: { video: { title: string } }) => (
    <div data-testid="video-card">{video.title}</div>
  ),
}));

import ChannelVideosList from "../ChannelVideosList";

describe("ChannelVideosList", () => {
  it("shows empty state when there are no videos", () => {
    render(<ChannelVideosList videos={[]} />);

    expect(screen.getByText(/No videos found/i)).toBeInTheDocument();
  });

  it("renders video cards", () => {
    render(<ChannelVideosList videos={[youtubeVideo("v-1"), youtubeVideo("v-2")]} />);

    expect(screen.getAllByTestId("video-card")).toHaveLength(2);
  });
});
