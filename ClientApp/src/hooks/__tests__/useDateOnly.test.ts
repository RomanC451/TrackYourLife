import { act, renderHook } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { useDateOnlyState } from "../useDateOnly";

describe("useDateOnlyState", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("initializes with today's date when no initial date is provided", () => {
    const { result } = renderHook(() => useDateOnlyState());

    expect(result.current[0]).toBe("2026-06-05");
  });

  it("initializes with a provided date", () => {
    const { result } = renderHook(() =>
      useDateOnlyState(new Date("2026-01-10T12:00:00Z")),
    );

    expect(result.current[0]).toBe("2026-01-10");
  });

  it("updates the stored date-only value", () => {
    const { result } = renderHook(() => useDateOnlyState());

    act(() => {
      result.current[1](new Date("2026-03-14T12:00:00Z"));
    });

    expect(result.current[0]).toBe("2026-03-14");
  });
});
