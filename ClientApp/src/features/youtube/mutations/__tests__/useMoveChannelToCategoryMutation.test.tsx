import { act, renderHook } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

const { mockMoveChannelToCategory } = vi.hoisted(() => ({
  mockMoveChannelToCategory: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockChannelsApi {
    moveChannelToCategory = mockMoveChannelToCategory;
  }
  return { ...actual, ChannelsApi: MockChannelsApi };
});

import useMoveChannelToCategoryMutation from "../useMoveChannelToCategoryMutation";

describe("useMoveChannelToCategoryMutation", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockMoveChannelToCategory.mockResolvedValue({ data: undefined });
  });

  it("moves a channel to another category", async () => {
    const { result } = renderHook(() => useMoveChannelToCategoryMutation(), {
      wrapper: createQueryClientWrapper(),
    });

    await act(async () => {
      await result.current.mutateAsync({
        youtubeChannelId: "ch-1",
        targetYoutubeCategoryId: "cat-2",
      });
    });

    expect(mockMoveChannelToCategory).toHaveBeenCalledWith("ch-1", {
      youtubeCategoryId: "cat-2",
    });
  });
});
