import { act, renderHook } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import useDelayedLoading, { useDelayedLoadingV2 } from "../useDelayedLoading";

describe("useDelayedLoading", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("starts in loaded state when not loading", () => {
    const { result } = renderHook(() => useDelayedLoading(false));

    expect(result.current).toEqual({
      isStarting: false,
      isLoading: false,
      isLoaded: true,
    });
  });

  it("enters starting state immediately when loading begins", () => {
    const { result } = renderHook(() => useDelayedLoading(true));

    expect(result.current).toEqual({
      isStarting: true,
      isLoading: false,
      isLoaded: false,
    });
  });

  it("shows loading after the delay elapses", () => {
    const { result } = renderHook(() => useDelayedLoading(true, 600));

    act(() => {
      vi.advanceTimersByTime(600);
    });

    expect(result.current).toEqual({
      isStarting: false,
      isLoading: true,
      isLoaded: false,
    });
  });

  it("returns to loaded state when loading stops", () => {
    const { result, rerender } = renderHook(
      ({ isLoading }) => useDelayedLoading(isLoading, 600),
      { initialProps: { isLoading: true as boolean | undefined } },
    );

    rerender({ isLoading: false });

    expect(result.current).toEqual({
      isStarting: false,
      isLoading: false,
      isLoaded: true,
    });
  });
});

describe("useDelayedLoadingV2", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("returns false before the delay and true after it", () => {
    const { result, rerender } = renderHook(
      ({ isLoading }) => useDelayedLoadingV2(isLoading, 300),
      { initialProps: { isLoading: false as boolean | undefined } },
    );

    rerender({ isLoading: true });
    expect(result.current).toBe(false);

    act(() => {
      vi.advanceTimersByTime(300);
    });

    expect(result.current).toBe(true);
  });
});
