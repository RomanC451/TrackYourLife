import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const trainingTemplatesUsageQueryKeys = {
  all: ["trainingTemplatesUsage"] as const,
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    [...trainingTemplatesUsageQueryKeys.all, startDate, endDate] as const,
};

export const trainingTemplatesUsageQueryOptions = {
  all: queryOptions({
    queryKey: trainingTemplatesUsageQueryKeys.all,
    queryFn: () =>
      trainingsApi.getTrainingTemplatesUsage().then((res) => res.data),
    placeholderData: keepPreviousData,
  }),
  byDateRange: (startDate: DateOnly | null, endDate: DateOnly | null) =>
    queryOptions({
      queryKey: trainingTemplatesUsageQueryKeys.byDateRange(startDate, endDate),
      queryFn: () =>
        trainingsApi
          .getTrainingTemplatesUsage(startDate, endDate)
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
