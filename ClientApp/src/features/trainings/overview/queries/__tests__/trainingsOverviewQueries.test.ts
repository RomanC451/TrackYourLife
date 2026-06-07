import { describe, expect, it } from "vitest";

import {
  allTrainingsOverviewKeys,
  caloriesBurnedHistoryQueryKeys,
  difficultyDistributionQueryKeys,
  exercisePerformanceQueryKeys,
  muscleGroupDistributionQueryKeys,
  topExercisesQueryKeys,
  trainingTemplatesUsageQueryKeys,
  trainingsOverviewQueryKeys,
  workoutDurationHistoryQueryKeys,
  workoutFrequencyQueryKeys,
} from "../trainingsOverviewQueries";

describe("trainingsOverviewQueryKeys", () => {
  it("includes the date range in the cache key", () => {
    expect(
      trainingsOverviewQueryKeys.byDateRange("2026-01-01", "2026-06-01"),
    ).toEqual(["trainingsOverview", "2026-01-01", "2026-06-01"]);
  });
});

describe("overview chart query keys", () => {
  it("encodes filter dimensions for workout frequency", () => {
    expect(
      workoutFrequencyQueryKeys.byFilters("2026-01-01", "2026-06-01", "Weekly"),
    ).toEqual([
      "workoutFrequency",
      "2026-01-01",
      "2026-06-01",
      "Weekly",
    ]);
  });

  it("encodes aggregation for calories burned history", () => {
    expect(
      caloriesBurnedHistoryQueryKeys.byFilters(
        "2026-01-01",
        "2026-06-01",
        "Weekly",
        "Sum",
      ),
    ).toEqual([
      "caloriesBurnedHistory",
      "2026-01-01",
      "2026-06-01",
      "Weekly",
      "Sum",
    ]);
  });

  it("encodes pagination for top exercises", () => {
    expect(topExercisesQueryKeys.byPage(2, 20)).toEqual([
      "topExercises",
      2,
      20,
    ]);
    expect(
      topExercisesQueryKeys.byFilters(2, 20, "2026-01-01", "2026-06-01"),
    ).toEqual(["topExercises", 2, 20, "2026-01-01", "2026-06-01"]);
  });

  it("encodes exercise performance filters", () => {
    expect(
      exercisePerformanceQueryKeys.byFilters(
        "2026-01-01",
        "2026-06-01",
        "ex-1",
        "Sequential",
        2,
        25,
      ),
    ).toEqual([
      "exercisePerformance",
      "2026-01-01",
      "2026-06-01",
      "ex-1",
      "Sequential",
      2,
      25,
    ]);
  });

  it("scopes distribution and template queries by date range", () => {
    expect(
      muscleGroupDistributionQueryKeys.byDateRange("2026-01-01", "2026-06-01"),
    ).toEqual([
      "muscleGroupDistribution",
      "2026-01-01",
      "2026-06-01",
      null,
    ]);
    expect(
      difficultyDistributionQueryKeys.byDateRange("2026-01-01", "2026-06-01"),
    ).toEqual(["difficultyDistribution", "2026-01-01", "2026-06-01"]);
    expect(
      trainingTemplatesUsageQueryKeys.byDateRange("2026-01-01", "2026-06-01"),
    ).toEqual(["trainingTemplatesUsage", "2026-01-01", "2026-06-01"]);
    expect(
      workoutDurationHistoryQueryKeys.byFilters(
        "2026-01-01",
        "2026-06-01",
        "Weekly",
        "Average",
      ),
    ).toEqual([
      "workoutDurationHistory",
      "2026-01-01",
      "2026-06-01",
      "Weekly",
      "Average",
    ]);
  });
});

describe("allTrainingsOverviewKeys", () => {
  it("lists every overview query root for bulk invalidation", () => {
    expect(allTrainingsOverviewKeys).toEqual([
      caloriesBurnedHistoryQueryKeys.all,
      difficultyDistributionQueryKeys.all,
      exercisePerformanceQueryKeys.all,
      muscleGroupDistributionQueryKeys.all,
      topExercisesQueryKeys.all,
      trainingsOverviewQueryKeys.all,
      trainingTemplatesUsageQueryKeys.all,
      workoutDurationHistoryQueryKeys.all,
      workoutFrequencyQueryKeys.all,
    ]);
  });
});
