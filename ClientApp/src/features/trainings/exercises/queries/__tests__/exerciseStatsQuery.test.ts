import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import type { ExerciseStatsDto } from "@/services/openapi";

import {
  exerciseStatsPlaceholderSameExercise,
  exerciseStatsQueryKeys,
  exerciseStatsSearchSchema,
  resolveExerciseStatsSearchFromParsedUrl,
} from "../exerciseStatsQuery";

describe("exerciseStatsSearchSchema", () => {
  it("applies defaults for an empty search object", () => {
    expect(exerciseStatsSearchSchema.parse({})).toEqual({
      range: "TwelveWeeks",
      chartMetric: "Volume",
    });
  });
});

describe("resolveExerciseStatsSearchFromParsedUrl", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("returns all-time search without dates", () => {
    expect(
      resolveExerciseStatsSearchFromParsedUrl({
        range: "All",
        chartMetric: "MaxWeight",
      }),
    ).toEqual({
      range: "All",
      chartMetric: "MaxWeight",
    });
  });

  it("fills missing dates from the default twelve-week window", () => {
    expect(
      resolveExerciseStatsSearchFromParsedUrl({
        range: "TwelveWeeks",
        chartMetric: "Volume",
      }),
    ).toEqual({
      range: "TwelveWeeks",
      chartMetric: "Volume",
      startDate: "2026-03-14",
      endDate: "2026-06-05",
    });
  });
});

describe("exerciseStatsPlaceholderSameExercise", () => {
  const stats = { exerciseId: "ex-1" } as unknown as ExerciseStatsDto;
  const placeholder = exerciseStatsPlaceholderSameExercise("ex-1");

  it("reuses data when the previous query is for the same exercise", () => {
    expect(
      placeholder(stats, {
        queryKey: exerciseStatsQueryKeys.bySearch("ex-1", {
          range: "All",
          chartMetric: "Volume",
        }),
      }),
    ).toBe(stats);
  });

  it("returns undefined for a different exercise id", () => {
    expect(
      placeholder(stats, {
        queryKey: exerciseStatsQueryKeys.bySearch("ex-2", {
          range: "All",
          chartMetric: "Volume",
        }),
      }),
    ).toBeUndefined();
  });
});
