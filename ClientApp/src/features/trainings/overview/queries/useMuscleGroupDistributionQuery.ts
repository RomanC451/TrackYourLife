import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

/** null = main groups only; string = subgroups of that muscle group. */
export type MuscleGroupFilter = string | null;

export const muscleGroupDistributionQueryKeys = {
  all: ["muscleGroupDistribution"] as const,
  byDateRange: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    muscleGroup: MuscleGroupFilter = null,
  ) =>
    [
      ...muscleGroupDistributionQueryKeys.all,
      startDate,
      endDate,
      muscleGroup,
    ] as const,
};

export const muscleGroupDistributionQueryOptions = {
  all: () =>
    queryOptions({
      queryKey: muscleGroupDistributionQueryKeys.byDateRange(null, null),
      queryFn: () =>
        trainingsApi.getMuscleGroupDistribution().then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
  byDateRange: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    muscleGroup: MuscleGroupFilter = null,
  ) =>
    queryOptions({
      queryKey: muscleGroupDistributionQueryKeys.byDateRange(
        startDate,
        endDate,
        muscleGroup,
      ),
      queryFn: () =>
        trainingsApi
          .getMuscleGroupDistribution(startDate, endDate, muscleGroup)
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
