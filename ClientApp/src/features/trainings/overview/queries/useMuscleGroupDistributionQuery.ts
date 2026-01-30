import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const muscleGroupDistributionQueryKeys = {
  all: ["muscleGroupDistribution"] as const,
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    [...muscleGroupDistributionQueryKeys.all, startDate, endDate] as const,
};

export const muscleGroupDistributionQueryOptions = {
  all: () =>
    queryOptions({
      queryKey: muscleGroupDistributionQueryKeys.byDateRange(null, null),
      queryFn: () =>
        trainingsApi.getMuscleGroupDistribution().then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    queryOptions({
      queryKey: muscleGroupDistributionQueryKeys.byDateRange(
        startDate,
        endDate,
      ),
      queryFn: () =>
        trainingsApi
          .getMuscleGroupDistribution(startDate, endDate)
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
