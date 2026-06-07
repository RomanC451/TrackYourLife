import { describe, expect, it } from "vitest";

import { workoutHistoryQueryKeys } from "../useWorkoutHistoryQuery";

describe("workoutHistoryQueryKeys", () => {
  it("scopes history by date range", () => {
    expect(
      workoutHistoryQueryKeys.byDateRange("2026-01-01", "2026-06-01"),
    ).toEqual(["workoutHistory", "2026-01-01", "2026-06-01"]);
    expect(workoutHistoryQueryKeys.byDateRange(null, null)).toEqual([
      "workoutHistory",
      null,
      null,
    ]);
  });
});
