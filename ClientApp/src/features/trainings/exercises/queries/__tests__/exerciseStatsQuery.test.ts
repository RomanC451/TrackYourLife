import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import type { ExerciseStatsDto } from "@/services/openapi";

const { mockGetExerciseStats } = vi.hoisted(() => ({
  mockGetExerciseStats: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockExercisesHistoriesApi {
    getExerciseStats = mockGetExerciseStats;
  }
  return { ...actual, ExercisesHistoriesApi: MockExercisesHistoriesApi };
});

import {
  defaultExerciseStatsDateWindow,
  ensureExerciseStatsData,
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

describe("defaultExerciseStatsDateWindow", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("covers the default twelve-week window", () => {
    expect(defaultExerciseStatsDateWindow()).toEqual({
      startDate: "2026-03-14",
      endDate: "2026-06-05",
    });
  });
});

describe("exerciseStatsQueryKeys.bySearch", () => {
  it("encodes null dates for all-time queries", () => {
    expect(
      exerciseStatsQueryKeys.bySearch("ex-1", {
        range: "All",
        chartMetric: "Volume",
      }),
    ).toEqual(["exerciseStats", "ex-1", "All", null, null, "Volume"]);
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

  it("returns undefined when previous data or query is missing", () => {
    expect(placeholder(undefined, undefined)).toBeUndefined();
    expect(
      placeholder(stats, { queryKey: ["exerciseStats", "ex-1"] }),
    ).toBeUndefined();
  });
});

describe("ensureExerciseStatsData", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    mockGetExerciseStats.mockResolvedValue({
      data: { exerciseId: "ex-1" },
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("parses search params and caches exercise stats", async () => {
    await ensureExerciseStatsData("ex-1", {
      range: "TwelveWeeks",
      chartMetric: "Volume",
    });

    expect(mockGetExerciseStats).toHaveBeenCalledWith(
      "ex-1",
      "TwelveWeeks",
      "Volume",
      "2026-03-14",
      "2026-06-05",
    );
    expect(
      queryClient.getQueryData(
        exerciseStatsQueryKeys.bySearch("ex-1", {
          range: "TwelveWeeks",
          chartMetric: "Volume",
          startDate: "2026-03-14",
          endDate: "2026-06-05",
        }),
      ),
    ).toEqual({ exerciseId: "ex-1" });
  });
});