import { renderHook } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { useDateRange } from "../useDateRange";

describe("useDateRange", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("defaults to the last 31 days through today", () => {
    const { result } = renderHook(() => useDateRange());

    expect(result.current.startDate).toBe("2026-05-05");
    expect(result.current.endDate).toBe("2026-06-05");
    expect(result.current.selectedRange?.from).toEqual(
      new Date("2026-05-05T12:00:00Z"),
    );
    expect(result.current.selectedRange?.to).toEqual(
      new Date("2026-06-05T12:00:00Z"),
    );
  });

  it("accepts explicit default dates", () => {
    const { result } = renderHook(() =>
      useDateRange({
        defaultStartDate: new Date("2026-01-01T00:00:00Z"),
        defaultEndDate: new Date("2026-03-01T00:00:00Z"),
      }),
    );

    expect(result.current.startDate).toBe("2026-01-01");
    expect(result.current.endDate).toBe("2026-03-01");
  });
});
