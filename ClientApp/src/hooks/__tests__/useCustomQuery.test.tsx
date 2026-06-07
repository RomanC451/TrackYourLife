import { renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { useCustomQuery } from "../useCustomQuery";

describe("useCustomQuery", () => {
  it("returns query data and delayed pending flags", async () => {
    const { result } = renderHook(
      () =>
        useCustomQuery({
          queryKey: ["custom-query"],
          queryFn: () => Promise.resolve("loaded"),
        }),
      { wrapper: createQueryClientWrapper() },
    );

    await waitFor(() => {
      expect(result.current.query.data).toBe("loaded");
    });

    expect(result.current.pendingState.isPending).toBe(false);
    expect(result.current.isDelayedPending).toBe(false);
  });

  it("calls onFirstFetch once when data arrives", async () => {
    const onFirstFetch = vi.fn();

    renderHook(
      () =>
        useCustomQuery({
          queryKey: ["custom-query-first-fetch"],
          queryFn: () => Promise.resolve("loaded"),
          onFirstFetch,
        }),
      { wrapper: createQueryClientWrapper() },
    );

    await waitFor(() => {
      expect(onFirstFetch).toHaveBeenCalledTimes(1);
    });
  });
});
