import { describe, expect, it } from "vitest";

import { formatDuration, formatDurationMs } from "../time";

describe("formatDuration", () => {
  it("formats minutes only when under one hour", () => {
    expect(formatDuration(0)).toBe("0 min");
    expect(formatDuration(45)).toBe("45 min");
  });

  it("formats hours and remaining minutes", () => {
    expect(formatDuration(60)).toBe("1 h  0 min");
    expect(formatDuration(90)).toBe("1 h  30 min");
    expect(formatDuration(125)).toBe("2 h  5 min");
  });
});

describe("formatDurationMs", () => {
  it("converts milliseconds to minutes before formatting", () => {
    expect(formatDurationMs(0)).toBe("0 min");
    expect(formatDurationMs(45 * 60_000)).toBe("45 min");
    expect(formatDurationMs(90 * 60_000)).toBe("1 h  30 min");
  });
});
