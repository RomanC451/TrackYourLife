import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeVideo } from "@/features/youtube/__tests__/fixtures";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockUseQuery } = vi.hoisted(() => ({
  mockUseQuery: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useQuery: (...args: unknown[]) => mockUseQuery(...args),
  };
});

vi.mock("@tanstack/react-router", () => ({
  Link: ({ children, to }: { children: React.ReactNode; to: string }) => (
    <a href={to}>{children}</a>
  ),
}));

vi.mock("../../videosList/VideoCard", () => ({
  default: ({ video }: { video: { title: string } }) => (
    <div data-testid="video-card">{video.title}</div>
  ),
}));

import HomeYoutubeRecommendation from "../HomeYoutubeRecommendation";

describe("HomeYoutubeRecommendation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows loading state", () => {
    mockUseQuery.mockReturnValue({ isPending: true });

    const { container } = render(<HomeYoutubeRecommendation />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(container.querySelector(".animate-spin")).toBeInTheDocument();
  });

  it("shows error state with retry", () => {
    const refetch = vi.fn();
    mockUseQuery.mockReturnValue({ isPending: false, isError: true, refetch });

    render(<HomeYoutubeRecommendation />, {
      wrapper: createQueryClientWrapper(),
    });

    fireEvent.click(screen.getByRole("button", { name: /Retry/i }));
    expect(refetch).toHaveBeenCalled();
  });

  it("shows empty recommendation state", () => {
    mockUseQuery.mockReturnValue({
      isPending: false,
      isError: false,
      data: null,
    });

    render(<HomeYoutubeRecommendation />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText(/No recommendations yet/i)).toBeInTheDocument();
    expect(screen.getByRole("link", { name: /Go to channels/i })).toHaveAttribute(
      "href",
      "/youtube/channels",
    );
  });

  it("renders a recommendation video", () => {
    mockUseQuery.mockReturnValue({
      isPending: false,
      isError: false,
      data: youtubeVideo("v-1"),
    });

    render(<HomeYoutubeRecommendation />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByTestId("video-card")).toHaveTextContent("Video v-1");
  });
});
