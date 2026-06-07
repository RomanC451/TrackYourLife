import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeVideo } from "@/features/youtube/__tests__/fixtures";

const { mockUseCustomQuery } = vi.hoisted(() => ({
  mockUseCustomQuery: vi.fn(),
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("../VideoCard", () => ({
  default: ({ video }: { video: { title: string } }) => (
    <div data-testid="video-card">{video.title}</div>
  ),
}));

import SearchVideosList from "../SearchVideosList";

describe("SearchVideosList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows an error state", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: undefined, isError: true },
    });

    render(<SearchVideosList searchQuery="squat" />);

    expect(
      screen.getByText(/error occurred while searching for videos/i),
    ).toBeInTheDocument();
  });

  it("shows empty results", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [], isError: false },
    });

    render(<SearchVideosList searchQuery="missing" />);

    expect(screen.getByText(/No videos found/i)).toBeInTheDocument();
  });

  it("renders search results", () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [youtubeVideo("v-1")], isError: false },
    });

    render(<SearchVideosList searchQuery="squat" />);

    expect(screen.getByTestId("video-card")).toHaveTextContent("Video v-1");
  });
});
