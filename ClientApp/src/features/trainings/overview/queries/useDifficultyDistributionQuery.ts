import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const difficultyDistributionQueryKeys = {
  all: ["difficultyDistribution"] as const,
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    [...difficultyDistributionQueryKeys.all, startDate, endDate] as const,
};

export const difficultyDistributionQueryOptions = {
  all: () =>
    queryOptions({
      queryKey: difficultyDistributionQueryKeys.byDateRange(null, null),
      queryFn: () =>
        trainingsApi.getDifficultyDistribution().then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    queryOptions({
      queryKey: difficultyDistributionQueryKeys.byDateRange(startDate, endDate),
      queryFn: () =>
        trainingsApi
          .getDifficultyDistribution(startDate, endDate)
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
