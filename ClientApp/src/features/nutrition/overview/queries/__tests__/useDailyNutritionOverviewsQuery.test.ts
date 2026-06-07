import { QueryClient } from "@tanstack/react-query";
import { describe, expect, it, vi } from "vitest";

import { queryClient } from "@/queryClient";
import { createQueryFnContext } from "@/test/queryFnContext";

import {
  dailyNutritionOverviewsQueryOptions,
  getNutritionSummaryDateRange,
  prefetchNutritionOverviewPageQueries,
} from "../useDailyNutritionOverviewsQuery";

const mockGetDailyNutritionOverviews = vi.hoisted(() => vi.fn());

vi.mock("@/services/openapi", async (importOriginal) => {
  const actual = await importOriginal<typeof import("@/services/openapi")>();
  class MockDailyNutritionOverviewsApi {
    getDailyNutritionOverviewsByDateRange = mockGetDailyNutritionOverviews;
  }
  return {
    ...actual,
    DailyNutritionOverviewsApi: MockDailyNutritionOverviewsApi,
  };
});

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

describe("dailyNutritionOverviewsQueryOptions.byDateRange", () => {
  it("returns placeholder data when no previous data exists", () => {
    const options = dailyNutritionOverviewsQueryOptions.byDateRange(
      "2026-06-01",
      "2026-06-07",
      "Daily",
      "Average",
    );

    const placeholder = (
      options.placeholderData as unknown as (
        previous: undefined,
      ) => { date: string; startDate: string; endDate: string }[]
    )(undefined);
    expect(placeholder).toHaveLength(1);
    expect(placeholder[0]).toMatchObject({
      date: "2026-06-01",
      startDate: "2026-06-01",
      endDate: "2026-06-07",
    });
  });

  it("keeps previous data as placeholder", () => {
    const options = dailyNutritionOverviewsQueryOptions.byDateRange(
      "2026-06-01",
      "2026-06-07",
      "Daily",
      "Average",
    );
    const previous = [{ id: "overview-1", date: "2026-06-01" }];

    expect(
      (options.placeholderData as (previous: unknown) => unknown)(
        previous as never,
      ),
    ).toBe(previous);
  });

  it("fetches overviews for the requested date range", async () => {
    mockGetDailyNutritionOverviews.mockResolvedValue({
      data: [{ id: "overview-1", date: "2026-06-01" }],
    });

    const options = dailyNutritionOverviewsQueryOptions.byDateRange(
      "2026-06-01",
      "2026-06-07",
      "Daily",
      "Sum",
    );

    const result = await options.queryFn!(
      createQueryFnContext({
        client: queryClient,
        queryKey: options.queryKey,
      }),
    );
    expect(result).toEqual([{ id: "overview-1", date: "2026-06-01" }]);
    expect(mockGetDailyNutritionOverviews).toHaveBeenCalledWith(
      "2026-06-01",
      "2026-06-07",
      "Daily",
      "Sum",
    );
  });
});

describe("prefetchNutritionOverviewPageQueries", () => {
  it("prefetches today and current week overview queries", async () => {
    const queryClient = new QueryClient();
    const prefetchSpy = vi.spyOn(queryClient, "prefetchQuery");

    await prefetchNutritionOverviewPageQueries(queryClient);

    expect(prefetchSpy).toHaveBeenCalledTimes(2);
  });
});
