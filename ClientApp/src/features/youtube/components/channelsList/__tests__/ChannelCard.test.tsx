import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeCategory, youtubeChannel } from "@/features/youtube/__tests__/fixtures";
import { setupYoutubeBrowserMocks } from "@/features/youtube/__tests__/setupBrowserMocks";

const { mockRemoveMutate, mockMoveMutate, mockFavoriteMutate } = vi.hoisted(() => ({
  mockRemoveMutate: vi.fn(),
  mockMoveMutate: vi.fn(),
  mockFavoriteMutate: vi.fn(),
}));

vi.mock("../../../mutations/useRemoveChannelMutation", () => ({
  default: () => ({ mutate: mockRemoveMutate, isPending: false }),
}));
vi.mock("../../../mutations/useMoveChannelToCategoryMutation", () => ({
  default: () => ({ mutate: mockMoveMutate, isPending: false }),
}));
vi.mock("../../../mutations/useSetChannelFavoriteMutation", () => ({
  default: () => ({ mutate: mockFavoriteMutate, isPending: false }),
}));
vi.mock("@tanstack/react-router", () => ({
  Link: ({ children }: { children: React.ReactNode }) => <a href="/channel">{children}</a>,
}));
vi.mock("@/components/ui/dropdown-menu", () => ({
  DropdownMenu: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  DropdownMenuTrigger: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  DropdownMenuContent: ({ children }: { children: React.ReactNode }) => <div>{children}</div>,
  DropdownMenuItem: ({
    children,
    onSelect,
  }: {
    children: React.ReactNode;
    onSelect?: () => void;
  }) => (
    <button type="button" onClick={() => onSelect?.()}>
      {children}
    </button>
  ),
}));

import ChannelCard from "../ChannelCard";

describe("ChannelCard", () => {
  beforeEach(() => {
    setupYoutubeBrowserMocks();
    vi.clearAllMocks();
  });

  it("renders channel details and toggles favorite", () => {
    render(
      <ChannelCard
        channel={youtubeChannel("ch-1")}
        categories={[
          youtubeCategory("cat-1"),
          youtubeCategory("cat-2", { name: "Learning" }),
        ]}
      />,
    );

    expect(screen.getByText("Channel ch-1")).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /Toggle favorite/i }));
    expect(mockFavoriteMutate).toHaveBeenCalledWith({
      youtubeChannelId: "ch-1",
      isFavorite: true,
    });
  });

  it("removes a channel", () => {
    render(
      <ChannelCard
        channel={youtubeChannel("ch-1")}
        categories={[youtubeCategory("cat-1")]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /Remove channel/i }));
    expect(mockRemoveMutate).toHaveBeenCalledWith({ youtubeChannelId: "ch-1" });
  });

  it("moves a channel to another category", () => {
    render(
      <ChannelCard
        channel={youtubeChannel("ch-1")}
        categories={[
          youtubeCategory("cat-1"),
          youtubeCategory("cat-2", { name: "Learning" }),
        ]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /Learning/i }));

    expect(mockMoveMutate).toHaveBeenCalledWith({
      youtubeChannelId: "ch-1",
      targetYoutubeCategoryId: "22222222-2222-4222-8222-222222222222",
    });
  });

  it("renders an initial when no thumbnail is available", () => {
    render(
      <ChannelCard
        channel={youtubeChannel("ch-1", { name: "Alpha", thumbnailUrl: undefined })}
        categories={[youtubeCategory("cat-1")]}
      />,
    );

    expect(screen.getByText("A")).toBeInTheDocument();
  });

  it("disables actions while a channel is deleting", () => {
    render(
      <ChannelCard
        channel={youtubeChannel("ch-1", { isDeleting: true })}
        categories={[youtubeCategory("cat-1")]}
      />,
    );

    expect(screen.getByRole("button", { name: /Toggle favorite/i })).toBeDisabled();
    expect(screen.getByRole("button", { name: /Remove channel/i })).toBeDisabled();
  });
});
