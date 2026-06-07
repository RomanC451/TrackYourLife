import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import type { TrainingStatsDto } from "@/services/openapi";

const { mockGetTrainingStats } = vi.hoisted(() => ({
  mockGetTrainingStats: vi.fn(),
}));

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockTrainingsApi {
    getTrainingStats = mockGetTrainingStats;
  }
  return { ...actual, TrainingsApi: MockTrainingsApi };
});

import {
  defaultTrainingStatsDateWindow,
  ensureTrainingStatsData,
  resolveTrainingStatsSearchFromParsedUrl,
  trainingStatsPlaceholderSameTraining,
  trainingStatsQueryKeys,
  trainingStatsSearchSchema,
} from "../trainingStatsQuery";
describe("trainingStatsSearchSchema", () => {
  it("applies defaults for an empty search object", () => {
    expect(trainingStatsSearchSchema.parse({})).toEqual({
      range: "TwelveWeeks",
      chartAggregation: "Sum",
    });
  });

  it("accepts explicit date range and aggregation", () => {
    expect(
      trainingStatsSearchSchema.parse({
        range: "FourWeeks",
        startDate: "2026-01-01",
        endDate: "2026-03-01",
        chartAggregation: "Average",
      }),
    ).toEqual({
      range: "FourWeeks",
      startDate: "2026-01-01",
      endDate: "2026-03-01",
      chartAggregation: "Average",
    });
  });
});

describe("resolveTrainingStatsSearchFromParsedUrl", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("returns all-time search without dates", () => {
    expect(
      resolveTrainingStatsSearchFromParsedUrl({
        range: "All",
        chartAggregation: "Sum",
      }),
    ).toEqual({
      range: "All",
      chartAggregation: "Sum",
    });
  });

  it("fills missing dates from the default twelve-week window", () => {
    expect(
      resolveTrainingStatsSearchFromParsedUrl({
        range: "TwelveWeeks",
        chartAggregation: "Average",
      }),
    ).toEqual({
      range: "TwelveWeeks",
      chartAggregation: "Average",
      startDate: "2026-03-14",
      endDate: "2026-06-05",
    });
  });

  it("keeps explicit custom dates", () => {
    expect(
      resolveTrainingStatsSearchFromParsedUrl({
        range: "SixMonths",
        startDate: "2026-01-01",
        endDate: "2026-06-01",
        chartAggregation: "Sum",
      }),
    ).toEqual({
      range: "SixMonths",
      chartAggregation: "Sum",
      startDate: "2026-01-01",
      endDate: "2026-06-01",
    });
  });
});

describe("trainingStatsQueryKeys.bySearch", () => {
  it("encodes null dates for all-time queries", () => {
    expect(
      trainingStatsQueryKeys.bySearch("training-1", {
        range: "All",
        chartAggregation: "Sum",
      }),
    ).toEqual([
      "trainingStats",
      "training-1",
      "All",
      null,
      null,
      "Sum",
    ]);
  });
});

describe("defaultTrainingStatsDateWindow", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("covers the default twelve-week window", () => {
    expect(defaultTrainingStatsDateWindow()).toEqual({
      startDate: "2026-03-14",
      endDate: "2026-06-05",
    });
  });
});

describe("trainingStatsPlaceholderSameTraining", () => {
  const stats = { trainingId: "training-1" } as unknown as TrainingStatsDto;
  const placeholder = trainingStatsPlaceholderSameTraining("training-1");

  it("reuses data when the previous query is for the same training", () => {
    expect(
      placeholder(stats, {
        queryKey: trainingStatsQueryKeys.bySearch("training-1", {
          range: "All",
          chartAggregation: "Sum",
        }),
      }),
    ).toBe(stats);
  });

  it("returns undefined for a different training id", () => {
    expect(
      placeholder(stats, {
        queryKey: trainingStatsQueryKeys.bySearch("training-2", {
          range: "All",
          chartAggregation: "Sum",
        }),
      }),
    ).toBeUndefined();
  });

  it("returns undefined when previous data or query is missing", () => {
    expect(placeholder(undefined, undefined)).toBeUndefined();
    expect(
      placeholder(stats, { queryKey: ["trainingStats", "training-1"] }),
    ).toBeUndefined();
  });
});

describe("ensureTrainingStatsData", () => {
  beforeEach(() => {
    queryClient.clear();
    vi.clearAllMocks();
    vi.useFakeTimers();
    vi.setSystemTime(new Date("2026-06-05T12:00:00Z"));
    mockGetTrainingStats.mockResolvedValue({
      data: { trainingId: "training-1" },
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("parses search params and caches training stats", async () => {
    await ensureTrainingStatsData("training-1", {
      range: "TwelveWeeks",
      chartAggregation: "Sum",
    });

    expect(mockGetTrainingStats).toHaveBeenCalledWith(
      "training-1",
      "TwelveWeeks",
      "Sum",
      "2026-03-14",
      "2026-06-05",
    );
    expect(
      queryClient.getQueryData(
        trainingStatsQueryKeys.bySearch("training-1", {
          range: "TwelveWeeks",
          chartAggregation: "Sum",
          startDate: "2026-03-14",
          endDate: "2026-06-05",
        }),
      ),
    ).toEqual({ trainingId: "training-1" });
  });
});