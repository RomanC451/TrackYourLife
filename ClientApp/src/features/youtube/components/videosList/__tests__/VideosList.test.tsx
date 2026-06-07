import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeVideo } from "@/features/youtube/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUseSuspenseQuery } = vi.hoisted(() => ({
  mockUseSuspenseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useSuspenseQuery: (...args: unknown[]) => mockUseSuspenseQuery(...args),
  };
});

vi.mock("../VideoCard", () => ({
  default: ({ video }: { video: { title: string } }) => (
    <div data-testid="video-card">{video.title}</div>
  ),
}));

import VideosList from "../VideosList";

describe("VideosList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows empty state when there are no videos", () => {
    mockUseSuspenseQuery.mockReturnValue({ data: [] });

    render(<VideosList categoryFilter="all" />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText(/No videos found/i)).toBeInTheDocument();
  });

  it("renders fetched videos", () => {
    mockUseSuspenseQuery.mockReturnValue({
      data: [youtubeVideo("v-1"), youtubeVideo("v-2")],
    });

    render(<VideosList categoryFilter="all" />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getAllByTestId("video-card")).toHaveLength(2);
  });
});
