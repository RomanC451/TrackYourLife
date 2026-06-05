import type { QueryClient } from "@tanstack/react-query";
import { queryOptions } from "@tanstack/react-query";
import {
  addDays,
  differenceInDays,
  endOfMonth,
  endOfWeek,
  startOfMonth,
  startOfWeek,
} from "date-fns";
import type { DateRange } from "react-day-picker";

import { getDateOnly, type DateOnly } from "@/lib/date";
import {
  AggregationMode,
  DailyNutritionOverviewDto,
  DailyNutritionOverviewsApi,
  OverviewType,
} from "@/services/openapi";

import { createEmptyNutritionalContent } from "../../common/utils/nutritionalContent";

const dailyNutritionOverviewsApi = new DailyNutritionOverviewsApi();

export const dailyNutritionOverviewsQueryKeys = {
  all: ["dailyNutritionOverviews"] as const,
  byDateRange: (
    startDate: DateOnly,
    endDate: DateOnly,
    overviewType: OverviewType,
    aggregationMode: AggregationMode,
  ) =>
    [
      ...dailyNutritionOverviewsQueryKeys.all,
      startDate,
      endDate,
      overviewType,
      aggregationMode,
    ] as const,
};

export const dailyNutritionOverviewsQueryOptions = {
  byDateRange: (
    startDate: DateOnly,
    endDate: DateOnly,
    overviewType: OverviewType,
    aggregationMode: AggregationMode,
  ) =>
    queryOptions({
      queryKey: dailyNutritionOverviewsQueryKeys.byDateRange(
        startDate,
        endDate,
        overviewType,
        aggregationMode,
      ),
      queryFn: () =>
        dailyNutritionOverviewsApi
          .getDailyNutritionOverviewsByDateRange(
            startDate,
            endDate,
            overviewType,
            aggregationMode,
          )
          .then((res) => res.data),

      placeholderData: (prev) =>
        prev ?? [
          {
            date: startDate,
            nutritionalContent: createEmptyNutritionalContent(),
            caloriesGoal: 0,
            carbohydratesGoal: 0,
            proteinGoal: 0,
            fatGoal: 0,
            isLoading: false,
            isDeleting: false,
            id: "",
            startDate: startDate,
            endDate: endDate,
          } as DailyNutritionOverviewDto,
        ],
    }),
};

/** Home + NutrientsCharts (Daily): today's totals with Sum aggregation. */
export function dailyNutritionOverviewTodaySumQueryOptions() {
  const today = getDateOnly(new Date());
  return dailyNutritionOverviewsQueryOptions.byDateRange(
    today,
    today,
    "Daily",
    "Sum",
  );
}

/** NutritionSummary initial state: current week, Daily overview, Average aggregation. */
export function dailyNutritionOverviewCurrentWeekDailyAverageQueryOptions(
  referenceDate = new Date(),
) {
  const { startDate, endDate } = getNutritionSummaryDateRange("Daily", {
    from: referenceDate,
    to: referenceDate,
  });
  return dailyNutritionOverviewsQueryOptions.byDateRange(
    startDate,
    endDate,
    "Daily",
    "Average",
  );
}

export function getNutritionSummaryDateRange(
  overviewType: OverviewType,
  selectedRange: DateRange | undefined,
): { startDate: DateOnly; endDate: DateOnly } {
  const from = selectedRange?.from ?? new Date();
  const to = selectedRange?.to ?? new Date();

  if (overviewType === "Daily") {
    return {
      startDate: getDateOnly(startOfWeek(from, { weekStartsOn: 1 })),
      endDate: getDateOnly(endOfWeek(to, { weekStartsOn: 1 })),
    };
  }

  if (overviewType === "Weekly") {
    return {
      startDate: getDateOnly(startOfMonth(from)),
      endDate: getDateOnly(endOfMonth(to)),
    };
  }

  const start = startOfMonth(from);
  let end = endOfMonth(to);
  if (differenceInDays(end, start) < 364) {
    end = addDays(start, 364);
  }

  return {
    startDate: getDateOnly(start),
    endDate: getDateOnly(end),
  };
}

export function prefetchNutritionOverviewPageQueries(queryClient: QueryClient) {
  return Promise.all([
    queryClient.prefetchQuery(dailyNutritionOverviewTodaySumQueryOptions()),
    queryClient.prefetchQuery(
      dailyNutritionOverviewCurrentWeekDailyAverageQueryOptions(),
    ),
  ]);
}
