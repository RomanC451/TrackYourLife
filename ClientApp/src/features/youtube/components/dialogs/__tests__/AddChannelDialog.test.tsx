import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeCategory } from "@/features/youtube/__tests__/fixtures";
import { setupYoutubeBrowserMocks } from "@/features/youtube/__tests__/setupBrowserMocks";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const {
  mockUseCustomQuery,
  mockNavigate,
  mockAddMutateAsync,
  mockRemoveMutateAsync,
} = vi.hoisted(() => ({
  mockUseCustomQuery: vi.fn(),
  mockNavigate: vi.fn(),
  mockAddMutateAsync: vi.fn(),
  mockRemoveMutateAsync: vi.fn(),
}));

vi.mock("@/contexts/AppGeneralContextProvider", () => ({
  useAppGeneralStateContext: () => ({ screenSize: { width: 1400 } }),
}));

vi.mock("@/hooks/useCustomQuery", () => ({
  useCustomQuery: (...args: unknown[]) => mockUseCustomQuery(...args),
}));

vi.mock("@tanstack/react-router", () => ({
  useNavigate: () => mockNavigate,
}));

vi.mock("../../../mutations/useAddChannelMutation", () => ({
  default: () => ({
    mutateAsync: mockAddMutateAsync,
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

vi.mock("../../../mutations/useRemoveChannelMutation", () => ({
  default: () => ({
    mutateAsync: mockRemoveMutateAsync,
    pendingState: { isPending: false, isDelayedPending: false },
  }),
}));

import AddChannelDialog from "../AddChannelDialog";

describe("AddChannelDialog", () => {
  beforeEach(() => {
    setupYoutubeBrowserMocks();
    vi.clearAllMocks();
    mockUseCustomQuery.mockReturnValue({
      query: { data: [], isFetching: false },
    });
  });

  it("prompts to create categories when none exist", () => {
    render(
      <AddChannelDialog categories={[]} defaultYoutubeCategoryId="cat-1" />,
      { wrapper: createQueryClientWrapper() },
    );

    expect(screen.getByText(/Create at least one category/i)).toBeInTheDocument();
  });

  it("searches for channels when categories exist", async () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: [
          {
            channelId: "ch-1",
            name: "Fitness Channel",
            thumbnailUrl: "https://example.com/thumb.jpg",
            subscriberCount: 1200,
            description: "Workouts",
            alreadySubscribed: false,
          },
        ],
        isFetching: false,
      },
    });

    render(
      <AddChannelDialog
        categories={[youtubeCategory("cat-1")]}
        defaultYoutubeCategoryId="cat-1"
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByPlaceholderText(/Search for channels/i), {
      target: { value: "fitness" },
    });

    await waitFor(() => {
      expect(screen.getByText("Fitness Channel")).toBeInTheDocument();
    });
  });

  it("removes a subscribed channel from search results", async () => {
    mockRemoveMutateAsync.mockResolvedValue(undefined);
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: [
          {
            channelId: "ch-subscribed",
            name: "Subscribed Channel",
            thumbnailUrl: "https://example.com/thumb.jpg",
            subscriberCount: 2500000,
            description: "Already added",
            alreadySubscribed: true,
          },
        ],
        isFetching: false,
      },
    });

    render(
      <AddChannelDialog
        categories={[youtubeCategory("cat-1")]}
        defaultYoutubeCategoryId="cat-1"
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByPlaceholderText(/Search for channels/i), {
      target: { value: "subscribed" },
    });

    await waitFor(() => {
      expect(screen.getByText("Subscribed Channel")).toBeInTheDocument();
    });

    expect(screen.getByText("2.5M subscribers")).toBeInTheDocument();
    expect(screen.getByText("Subscribed")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /Remove/i }));

    await waitFor(() => {
      expect(mockRemoveMutateAsync).toHaveBeenCalledWith({
        youtubeChannelId: "ch-subscribed",
      });
    });
  });

  it("formats subscriber counts for smaller channels", async () => {
    mockUseCustomQuery.mockReturnValue({
      query: {
        data: [
          {
            channelId: "ch-small",
            name: "Small Channel",
            thumbnailUrl: "https://example.com/thumb.jpg",
            subscriberCount: 850,
            description: "Niche content",
            alreadySubscribed: false,
          },
          {
            channelId: "ch-medium",
            name: "Medium Channel",
            thumbnailUrl: "https://example.com/thumb.jpg",
            subscriberCount: 4500,
            description: "Growing audience",
            alreadySubscribed: false,
          },
        ],
        isFetching: false,
      },
    });

    render(
      <AddChannelDialog
        categories={[youtubeCategory("cat-1")]}
        defaultYoutubeCategoryId="cat-1"
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByPlaceholderText(/Search for channels/i), {
      target: { value: "channel" },
    });

    await waitFor(() => {
      expect(screen.getByText("850 subscribers")).toBeInTheDocument();
      expect(screen.getByText("4.5K subscribers")).toBeInTheDocument();
    });
  });

  it("shows an empty state when no channels match the search", async () => {
    mockUseCustomQuery.mockReturnValue({
      query: { data: [], isFetching: false },
    });

    render(
      <AddChannelDialog
        categories={[youtubeCategory("cat-1")]}
        defaultYoutubeCategoryId="cat-1"
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.change(screen.getByPlaceholderText(/Search for channels/i), {
      target: { value: "missing" },
    });

    await waitFor(() => {
      expect(screen.getByText(/No channels found for/i)).toBeInTheDocument();
    });
  });

  it("calls onClose when the dialog is dismissed", () => {
    const onClose = vi.fn();

    render(
      <AddChannelDialog
        categories={[youtubeCategory("cat-1")]}
        defaultYoutubeCategoryId="cat-1"
        onClose={onClose}
      />,
      { wrapper: createQueryClientWrapper() },
    );

    fireEvent.keyDown(document, { key: "Escape" });
    expect(onClose).toHaveBeenCalled();
  });
});
