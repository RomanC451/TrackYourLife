import { queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { OverviewType2, TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const trainingsOverviewQueryKeys = {
  all: ["trainingsOverview"] as const,
  byDateRange: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: string,
  ) =>
    [...trainingsOverviewQueryKeys.all, startDate, endDate, overviewType] as const,
};

export const trainingsOverviewQueryOptions = {
  byDateRange: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    overviewType: string = "Daily",
  ) =>
    queryOptions({
      queryKey: trainingsOverviewQueryKeys.byDateRange(
        startDate,
        endDate,
        overviewType,
      ),
      queryFn: () =>
        trainingsApi
          .getTrainingsOverview(
            overviewType as OverviewType2,
            startDate,
            endDate,
          )
          .then((res) => res.data),
    }),
};
