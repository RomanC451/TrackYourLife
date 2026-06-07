import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import {
  isYoutubeCardClickSuppressed,
  suppressYoutubeCardClick,
} from "../youtubeClickGuard";

describe("youtubeClickGuard", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("suppresses card clicks for a short window", () => {
    expect(isYoutubeCardClickSuppressed()).toBe(false);

    suppressYoutubeCardClick(500);
    expect(isYoutubeCardClickSuppressed()).toBe(true);

    vi.advanceTimersByTime(499);
    expect(isYoutubeCardClickSuppressed()).toBe(true);

    vi.advanceTimersByTime(1);
    expect(isYoutubeCardClickSuppressed()).toBe(false);
  });
});
