import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeChannel, youtubeSettings } from "@/features/youtube/__tests__/fixtures";
import { setupYoutubeBrowserMocks } from "@/features/youtube/__tests__/setupBrowserMocks";

const {
  mockUseQuery,
  mockRemoveMutate,
  mockFavoriteMutate,
  mockMoveMutate,
  mockSubscribeDropdown,
} = vi.hoisted(() => ({
  mockUseQuery: vi.fn(),
  mockRemoveMutate: vi.fn(),
  mockFavoriteMutate: vi.fn(),
  mockMoveMutate: vi.fn(),
  mockSubscribeDropdown: vi.fn(),
}));

vi.mock("@tanstack/react-query", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@tanstack/react-query")>();
  return {
    ...actual,
    useQuery: (...args: unknown[]) => mockUseQuery(...args),
  };
});
vi.mock("../../channels/SubscribeChannelDropdown", () => ({
  default: (props: { channelName: string }) => {
    mockSubscribeDropdown(props);
    return <div data-testid="subscribe-dropdown">{props.channelName}</div>;
  },
}));
vi.mock("../../../mutations/useRemoveChannelMutation", () => ({
  default: () => ({ mutate: mockRemoveMutate, isPending: false }),
}));
vi.mock("../../../mutations/useMoveChannelToCategoryMutation", () => ({
  default: () => ({ mutate: mockMoveMutate, isPending: false }),
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
vi.mock("../../../mutations/useSetChannelFavoriteMutation", () => ({
  default: () => ({ mutate: mockFavoriteMutate, isPending: false }),
}));

import ChannelHeaderActions from "../ChannelHeaderActions";

describe("ChannelHeaderActions", () => {
  beforeEach(() => {
    setupYoutubeBrowserMocks();
    vi.clearAllMocks();
    mockUseQuery.mockReturnValue({ data: youtubeSettings() });
  });

  it("shows subscribe dropdown for unsubscribed channels", () => {
    render(
      <ChannelHeaderActions
        youtubeChannelId="ch-1"
        channelName="Fitness Channel"
        subscribedChannel={undefined}
      />,
    );

    expect(screen.getByTestId("subscribe-dropdown")).toHaveTextContent(
      "Fitness Channel",
    );
  });

  it("shows favorite and remove actions for subscribed channels", () => {
    render(
      <ChannelHeaderActions
        youtubeChannelId="ch-1"
        channelName="Fitness Channel"
        subscribedChannel={youtubeChannel("ch-1")}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /Favorite/i }));
    fireEvent.click(screen.getByRole("button", { name: /Remove/i }));

    expect(mockFavoriteMutate).toHaveBeenCalled();
    expect(mockRemoveMutate).toHaveBeenCalledWith({ youtubeChannelId: "ch-1" });
  });

  it("moves a channel to another category", () => {
    render(
      <ChannelHeaderActions
        youtubeChannelId="ch-1"
        channelName="Fitness Channel"
        subscribedChannel={youtubeChannel("ch-1")}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /Learning/i }));

    expect(mockMoveMutate).toHaveBeenCalledWith({
      youtubeChannelId: "ch-1",
      targetYoutubeCategoryId: "22222222-2222-4222-8222-222222222222",
    });
  });

  it("toggles favorite off for favorited channels", () => {
    render(
      <ChannelHeaderActions
        youtubeChannelId="ch-1"
        channelName="Fitness Channel"
        subscribedChannel={youtubeChannel("ch-1", { isFavorite: true })}
      />,
    );

    expect(screen.getByRole("button", { name: /Favorited/i })).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: /Favorited/i }));

    expect(mockFavoriteMutate).toHaveBeenCalledWith({
      youtubeChannelId: "ch-1",
      isFavorite: false,
    });
  });

  it("disables actions while a channel is deleting", () => {
    render(
      <ChannelHeaderActions
        youtubeChannelId="ch-1"
        channelName="Fitness Channel"
        subscribedChannel={youtubeChannel("ch-1", { isDeleting: true })}
      />,
    );

    expect(screen.getByRole("button", { name: /^Favorite$/i })).toBeDisabled();
    expect(screen.getByRole("button", { name: /Remove/i })).toBeDisabled();
  });
});
