import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { wait } from "../wait";

describe("wait", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("resolves after the requested number of seconds", async () => {
    const promise = wait(2);
    const resolved = vi.fn();

    promise.then(resolved);
    expect(resolved).not.toHaveBeenCalled();

    await vi.advanceTimersByTimeAsync(2000);
    expect(resolved).toHaveBeenCalledTimes(1);
  });
});
