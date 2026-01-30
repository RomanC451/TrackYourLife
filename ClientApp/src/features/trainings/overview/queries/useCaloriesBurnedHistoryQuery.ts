import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import {
  AggregationType,
  OverviewType2,
  TrainingsApi,
} from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const caloriesBurnedHistoryQueryKeys = {
  all: ["caloriesBurnedHistory"] as const,
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2,
    aggregationType: AggregationType,
  ) =>
    [
      ...caloriesBurnedHistoryQueryKeys.all,
      startDate,
      endDate,
      overviewType,
      aggregationType,
    ] as const,
};

export const caloriesBurnedHistoryQueryOptions = {
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2 = "Weekly",
    aggregationType: AggregationType = "Sum",
  ) =>
    queryOptions({
      queryKey: caloriesBurnedHistoryQueryKeys.byFilters(
        startDate,
        endDate,
        overviewType,
        aggregationType,
      ),
      queryFn: () =>
        trainingsApi
          .getCaloriesBurnedHistory(
            overviewType,
            aggregationType,
            startDate,
            endDate,
          )
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
