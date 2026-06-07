import { beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";

const { mockGetWatchHistory } = vi.hoisted(() => ({
  mockGetWatchHistory: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockVideosApi {
    getWatchHistory = mockGetWatchHistory;
  }
  return { ...actual, VideosApi: MockVideosApi };
});

import {
  watchHistoryQueryKeys,
  watchHistoryQueryOptions,
  WATCH_HISTORY_PAGE_SIZE,
} from "../useWatchHistoryQuery";

describe("useWatchHistoryQuery", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("defines watch history query keys and page size", () => {
    expect(watchHistoryQueryKeys.all).toEqual(["youtube", "watchHistory"]);
    expect(WATCH_HISTORY_PAGE_SIZE).toBe(20);
  });

  it("fetches paginated watch history", async () => {
    mockGetWatchHistory.mockResolvedValue({
      data: { items: [], hasNextPage: true, page: 1 },
    });

    const options = watchHistoryQueryOptions.infinite();
    const getNextPageParam = options.getNextPageParam as (
      lastPage: { hasNextPage: boolean; page: number },
    ) => number | undefined;

    const page = await options.queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: options.queryKey,
        pageParam: 1,
        direction: "forward",
      }),
    );

    expect(mockGetWatchHistory).toHaveBeenCalledWith(1, 20);
    expect(page).toEqual({ items: [], hasNextPage: true, page: 1 });
    expect(getNextPageParam(page)).toBe(2);
    expect(
      getNextPageParam({ ...page, hasNextPage: false }),
    ).toBeUndefined();
  });
});
