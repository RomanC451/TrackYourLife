import type { QueryClient } from "@tanstack/react-query";
import { describe, expect, it, vi } from "vitest";

import { prefetchTrainingsOverviewPageQueries } from "../trainingsOverviewQueries";

describe("prefetchTrainingsOverviewPageQueries", () => {
  it("prefetches all overview page queries for a date range", async () => {
    const ensureQueryData = vi.fn().mockResolvedValue(undefined);
    const queryClient = { ensureQueryData } as unknown as QueryClient;

    await prefetchTrainingsOverviewPageQueries(
      queryClient,
      "2026-05-05",
      "2026-06-05",
    );

    expect(ensureQueryData).toHaveBeenCalledTimes(9);
  });
});
