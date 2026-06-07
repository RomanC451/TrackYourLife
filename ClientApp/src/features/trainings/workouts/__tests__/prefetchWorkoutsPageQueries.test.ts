import type { QueryClient } from "@tanstack/react-query";
import { describe, expect, it, vi } from "vitest";

import { prefetchWorkoutsPageQueries } from "../prefetchWorkoutsPageQueries";

describe("prefetchWorkoutsPageQueries", () => {
  it("prefetches all workouts page queries", async () => {
    const ensureQueryData = vi.fn().mockResolvedValue(undefined);
    const queryClient = { ensureQueryData } as unknown as QueryClient;

    await prefetchWorkoutsPageQueries(queryClient);

    expect(ensureQueryData).toHaveBeenCalledTimes(8);
  });

  it("logs failures in development without throwing", async () => {
    const consoleError = vi.spyOn(console, "error").mockImplementation(() => {});
    const ensureQueryData = vi
      .fn()
      .mockResolvedValueOnce(undefined)
      .mockRejectedValueOnce(new Error("network"))
      .mockResolvedValue(undefined);
    const queryClient = { ensureQueryData } as unknown as QueryClient;

    vi.stubEnv("DEV", true);
    await expect(
      prefetchWorkoutsPageQueries(queryClient),
    ).resolves.toBeUndefined();
    expect(consoleError).toHaveBeenCalled();

    consoleError.mockRestore();
    vi.unstubAllEnvs();
  });
});
