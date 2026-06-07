import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import {
  dailyNutritionOverviewCurrentWeekDailyAverageQueryOptions,
  dailyNutritionOverviewTodaySumQueryOptions,
  dailyNutritionOverviewsQueryKeys,
} from "../useDailyNutritionOverviewsQuery";

describe("dailyNutritionOverviewsQueryKeys", () => {
  it("includes date range, overview type, and aggregation mode", () => {
    expect(
      dailyNutritionOverviewsQueryKeys.byDateRange(
        "2026-06-01",
        "2026-06-07",
        "Daily",
        "Average",
      ),
    ).toEqual([
      "dailyNutritionOverviews",
      "2026-06-01",
      "2026-06-07",
      "Daily",
      "Average",
    ]);
  });
});

describe("dailyNutritionOverviewTodaySumQueryOptions", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("queries today's daily overview with sum aggregation", () => {
    const options = dailyNutritionOverviewTodaySumQueryOptions();

    expect(options.queryKey).toEqual([
      "dailyNutritionOverviews",
      "2026-06-05",
      "2026-06-05",
      "Daily",
      "Sum",
    ]);
  });
});

describe("dailyNutritionOverviewCurrentWeekDailyAverageQueryOptions", () => {
  it("queries the current week with daily average aggregation", () => {
    const options =
      dailyNutritionOverviewCurrentWeekDailyAverageQueryOptions(
        new Date("2026-06-05"),
      );

    expect(options.queryKey).toEqual([
      "dailyNutritionOverviews",
      "2026-06-01",
      "2026-06-07",
      "Daily",
      "Average",
    ]);
  });
});
