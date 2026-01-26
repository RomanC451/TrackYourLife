import { queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const workoutHistoryQueryKeys = {
  all: ["workoutHistory"] as const,
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    [...workoutHistoryQueryKeys.all, startDate, endDate] as const,
};

export const workoutHistoryQueryOptions = {
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    queryOptions({
      queryKey: workoutHistoryQueryKeys.byDateRange(startDate, endDate),
      queryFn: () =>
        trainingsApi
          .getWorkoutHistory(startDate , endDate)
          .then((res) => res.data),
    }),
};
