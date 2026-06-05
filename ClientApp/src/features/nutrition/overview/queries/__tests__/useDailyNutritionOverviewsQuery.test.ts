import { describe, expect, it } from "vitest";

import { getNutritionSummaryDateRange } from "../useDailyNutritionOverviewsQuery";

describe("getNutritionSummaryDateRange", () => {
  it("returns the current week for daily overview", () => {
    expect(
      getNutritionSummaryDateRange("Daily", {
        from: new Date("2026-06-05"),
        to: new Date("2026-06-05"),
      }),
    ).toEqual({
      startDate: "2026-06-01",
      endDate: "2026-06-07",
    });
  });

  it("returns the current month for weekly overview", () => {
    expect(
      getNutritionSummaryDateRange("Weekly", {
        from: new Date("2026-06-15"),
        to: new Date("2026-06-15"),
      }),
    ).toEqual({
      startDate: "2026-06-01",
      endDate: "2026-06-30",
    });
  });

  it("extends monthly overview to at least 364 days", () => {
    expect(
      getNutritionSummaryDateRange("Monthly", {
        from: new Date("2026-02-01"),
        to: new Date("2026-02-28"),
      }),
    ).toEqual({
      startDate: "2026-02-01",
      endDate: "2027-01-31",
    });
  });
});
