import { act, renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import useFoodSearch from "../useFoodSearchQuery";

const mockUseCustomInfiniteQuery = vi.fn();

vi.mock("@/hooks/useCustomInfiniteQuery", () => ({
  useCustomInfiniteQuery: (...args: unknown[]) =>
    mockUseCustomInfiniteQuery(...args),
}));

describe("useFoodSearch", () => {
  it("tracks the search value and exposes query state", () => {
    mockUseCustomInfiniteQuery.mockReturnValue({
      query: { data: { pages: [] } },
      pendingState: { isPending: false },
    });

    const { result } = renderHook(() => useFoodSearch());

    expect(result.current.searchValue).toBe("");
    expect(result.current.error).toBe("");

    act(() => {
      result.current.setSearchValue("oats");
    });

    expect(result.current.searchValue).toBe("oats");
    expect(mockUseCustomInfiniteQuery).toHaveBeenCalled();
  });

  it("clears the error with resetError", () => {
    mockUseCustomInfiniteQuery.mockReturnValue({
      query: { data: undefined },
      pendingState: { isPending: true },
    });

    const { result } = renderHook(() => useFoodSearch());

    act(() => {
      result.current.setSearchValue("bad");
    });

    act(() => {
      result.current.resetError();
    });

    expect(result.current.error).toBe("");
  });
});
