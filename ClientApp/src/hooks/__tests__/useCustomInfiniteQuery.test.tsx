import { renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { useCustomInfiniteQuery } from "../useCustomInfiniteQuery";

describe("useCustomInfiniteQuery", () => {
  it("returns infinite query data and delayed pending flags", async () => {
    const { result } = renderHook(
      () =>
        useCustomInfiniteQuery({
          queryKey: ["custom-infinite-query"],
          queryFn: ({ pageParam }) => Promise.resolve([`page-${pageParam}`]),
          initialPageParam: 0,
          getNextPageParam: (_lastPage, _allPages, lastPageParam) =>
            lastPageParam < 1 ? lastPageParam + 1 : undefined,
        }),
      { wrapper: createQueryClientWrapper() },
    );

    await waitFor(() => {
      expect(
        (result.current.query.data as { pages: string[][] } | undefined)?.pages[0],
      ).toEqual(["page-0"]);
    });

    expect(result.current.pendingState.isPending).toBe(false);
    expect(result.current.isDelayedPending).toBe(false);
  });

  it("calls onFirstFetch once when data arrives", async () => {
    const onFirstFetch = vi.fn();

    renderHook(
      () =>
        useCustomInfiniteQuery({
          queryKey: ["custom-infinite-first-fetch"],
          queryFn: ({ pageParam }) => Promise.resolve([`page-${pageParam}`]),
          initialPageParam: 0,
          getNextPageParam: () => undefined,
          onFirstFetch,
        }),
      { wrapper: createQueryClientWrapper() },
    );

    await waitFor(() => {
      expect(onFirstFetch).toHaveBeenCalledTimes(1);
    });
  });
});
