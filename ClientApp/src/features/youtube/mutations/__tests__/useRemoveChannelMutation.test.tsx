import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeChannel } from "@/features/youtube/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockRemoveChannel, mockToastError } = vi.hoisted(() => ({
  mockRemoveChannel: vi.fn(),
  mockToastError: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockChannelsApi {
    removeChannel = mockRemoveChannel;
  }
  return { ...actual, ChannelsApi: MockChannelsApi };
});

vi.mock("sonner", () => ({
  toast: {
    error: mockToastError,
  },
}));

import useRemoveChannelMutation from "../useRemoveChannelMutation";
import { youtubeQueryKeys } from "../../queries/youtubeQueries";

describe("useRemoveChannelMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    queryClient.clear();
    mockRemoveChannel.mockResolvedValue({ data: undefined });
  });

  it("marks a channel as deleting optimistically", async () => {
    queryClient.setQueryData(youtubeQueryKeys.channels("all"), [
      youtubeChannel("ch-1"),
    ]);

    let resolveRemove: (value: unknown) => void = () => undefined;
    mockRemoveChannel.mockImplementation(
      () =>
        new Promise((resolve) => {
          resolveRemove = resolve;
        }),
    );

    const { result } = renderHook(() => useRemoveChannelMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      result.current.mutate({ youtubeChannelId: "ch-1" });
      await Promise.resolve();
    });

    expect(
      queryClient
        .getQueryData<ReturnType<typeof youtubeChannel>[]>(
          youtubeQueryKeys.channels("all"),
        )?.[0]?.isDeleting,
    ).toBe(true);

    await act(async () => {
      resolveRemove({ data: undefined });
      await Promise.resolve();
    });

    expect(mockRemoveChannel).toHaveBeenCalledWith("ch-1");
  });

  it("rolls back optimistic updates when removal fails", async () => {
    const original = [youtubeChannel("ch-1")];
    queryClient.setQueryData(youtubeQueryKeys.channels("all"), original);
    mockRemoveChannel.mockRejectedValue({
      response: { data: { detail: "Not found" } },
    });

    const { result } = renderHook(() => useRemoveChannelMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({ youtubeChannelId: "ch-1" }).catch(() => undefined);
    });

    expect(queryClient.getQueryData(youtubeQueryKeys.channels("all"))).toEqual(
      original,
    );
    expect(mockToastError).toHaveBeenCalledWith("Not found");
  });
});
