import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import {
  AggregationType,
  OverviewType2,
  TrainingsApi,
} from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const workoutDurationHistoryQueryKeys = {
  all: ["workoutDurationHistory"] as const,
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2,
    aggregationType: AggregationType,
  ) =>
    [
      ...workoutDurationHistoryQueryKeys.all,
      startDate,
      endDate,
      overviewType,
      aggregationType,
    ] as const,
};

export const workoutDurationHistoryQueryOptions = {
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2 = "Weekly",
    aggregationType: AggregationType = "Sum",
  ) =>
    queryOptions({
      queryKey: workoutDurationHistoryQueryKeys.byFilters(
        startDate,
        endDate,
        overviewType,
        aggregationType,
      ),
      queryFn: () =>
        trainingsApi
          .getWorkoutDurationHistory(
            overviewType,
            aggregationType,
            startDate,
            endDate,
          )
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
