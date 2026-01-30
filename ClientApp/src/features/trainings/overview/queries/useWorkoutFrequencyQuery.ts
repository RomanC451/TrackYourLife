import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { OverviewType2, TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const workoutFrequencyQueryKeys = {
  all: ["workoutFrequency"] as const,
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2,
  ) =>
    [
      ...workoutFrequencyQueryKeys.all,
      startDate,
      endDate,
      overviewType,
    ] as const,
};

export const workoutFrequencyQueryOptions = {
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: OverviewType2 = "Daily",
  ) =>
    queryOptions({
      queryKey: workoutFrequencyQueryKeys.byFilters(
        startDate,
        endDate,
        overviewType,
      ),
      queryFn: () =>
        trainingsApi
          .getWorkoutFrequency(overviewType, startDate, endDate)
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
