import { renderHook } from "@testing-library/react";
import type { ReactNode } from "react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import {
  OverviewDateRangeProvider,
  useOverviewDateRange,
} from "../OverviewDateRangeContext";

function wrapper({ children }: { children: ReactNode }) {
  return <OverviewDateRangeProvider>{children}</OverviewDateRangeProvider>;
}

describe("useOverviewDateRange", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("exposes the date range from the provider", () => {
    const { result } = renderHook(() => useOverviewDateRange(), { wrapper });

    expect(result.current.startDate).toBe("2026-05-05");
    expect(result.current.endDate).toBe("2026-06-05");
  });

  it("throws when used outside the provider", () => {
    expect(() => renderHook(() => useOverviewDateRange())).toThrow(
      "useOverviewDateRange must be used within OverviewDateRangeProvider",
    );
  });
});
