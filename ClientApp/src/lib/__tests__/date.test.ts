import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { getDateOnly, parseDateOnly } from "../date";

describe("getDateOnly", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("formats a date as yyyy-MM-dd", () => {
    expect(getDateOnly(new Date("2026-06-05T12:00:00Z"))).toBe("2026-06-05");
  });
});

describe("parseDateOnly", () => {
  it("parses a date-only string into a Date", () => {
    const parsed = parseDateOnly("2026-06-05");
    expect(parsed).toBeInstanceOf(Date);
    expect(parsed.toISOString()).toContain("2026-06-05");
  });
});
