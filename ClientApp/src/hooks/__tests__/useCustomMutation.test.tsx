import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it } from "vitest";

import { createQueryClientWrapper } from "@/test/queryClientWrapper";

import { useCustomMutation } from "../useCustomMutation";

describe("useCustomMutation", () => {
  it("returns mutation result with delayed pending state", async () => {
    const { result } = renderHook(
      () =>
        useCustomMutation({
          mutationFn: async (value: string) => `ok:${value}`,
        }),
      { wrapper: createQueryClientWrapper() },
    );

    act(() => {
      result.current.mutate("test");
    });

    await waitFor(() => {
      expect(result.current.data).toBe("ok:test");
    });

    expect(result.current.pendingState.isPending).toBe(false);
    expect(result.current.isDelayedPending).toBe(false);
  });
});
