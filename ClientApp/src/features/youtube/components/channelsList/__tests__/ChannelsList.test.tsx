import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeCategory, youtubeChannel } from "@/features/youtube/__tests__/fixtures";
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

vi.mock("../ChannelCard", () => ({
  default: ({ channel }: { channel: { name: string } }) => (
    <div data-testid="channel-card">{channel.name}</div>
  ),
}));

import ChannelsList from "../ChannelsList";

describe("ChannelsList", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows empty state when there are no channels", () => {
    mockUseSuspenseQuery.mockReturnValue({ data: [] });

    render(<ChannelsList categoryFilter="all" categories={[youtubeCategory("cat-1")]} />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getByText(/No channels found/i)).toBeInTheDocument();
  });

  it("renders fetched channels", () => {
    mockUseSuspenseQuery.mockReturnValue({
      data: [youtubeChannel("ch-1"), youtubeChannel("ch-2")],
    });

    render(<ChannelsList categoryFilter="all" categories={[youtubeCategory("cat-1")]} />, {
      wrapper: createQueryClientWrapper(),
    });

    expect(screen.getAllByTestId("channel-card")).toHaveLength(2);
  });
});
