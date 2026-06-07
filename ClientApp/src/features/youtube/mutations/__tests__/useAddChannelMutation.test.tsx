import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { youtubeChannel } from "@/features/youtube/__tests__/fixtures";
import { queryClient } from "@/queryClient";
import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockAddChannelToCategory } = vi.hoisted(() => ({
  mockAddChannelToCategory: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockChannelsApi {
    addChannelToCategory = mockAddChannelToCategory;
  }
  return { ...actual, ChannelsApi: MockChannelsApi };
});

import useAddChannelMutation from "../useAddChannelMutation";
import { youtubeQueryKeys } from "../../queries/youtubeQueries";

describe("useAddChannelMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    queryClient.clear();
    mockAddChannelToCategory.mockResolvedValue({ data: undefined });
  });

  it("optimistically appends a channel to the list", async () => {
    queryClient.setQueryData(youtubeQueryKeys.channels("cat-1"), [
      youtubeChannel("existing"),
    ]);

    const { result } = renderHook(() => useAddChannelMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        youtubeChannelId: "new-channel",
        youtubeCategoryId: "cat-1",
        channelName: "New Channel",
        categoryName: "Fitness",
      });
    });

    expect(mockAddChannelToCategory).toHaveBeenCalledWith({
      youtubeChannelId: "new-channel",
      youtubeCategoryId: "cat-1",
    });
    expect(
      queryClient.getQueryData<ReturnType<typeof youtubeChannel>[]>(
        youtubeQueryKeys.channels("cat-1"),
      ),
    ).toHaveLength(2);
  });
});
