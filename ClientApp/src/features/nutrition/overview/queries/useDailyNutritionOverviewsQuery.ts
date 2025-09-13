import { queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { DailyNutritionOverviewsApi } from "@/services/openapi";

const dailyNutritionOverviewsApi = new DailyNutritionOverviewsApi();

export const dailyNutritionOverviewsQueryKeys = {
  all: ["dailyNutritionOverviews"] as const,
  byDateRange: (startDate: DateOnly, endDate: DateOnly) =>
    [...dailyNutritionOverviewsQueryKeys.all, startDate, endDate] as const,
};

export const dailyNutritionOverviewsQueryOptions = {
  byDateRange: (startDate: DateOnly, endDate: DateOnly) =>
    queryOptions({
      queryKey: dailyNutritionOverviewsQueryKeys.byDateRange(
        startDate,
        endDate,
      ),
      queryFn: () =>
        dailyNutritionOverviewsApi
          .getDailyNutritionOverviewsByDateRange(startDate, endDate)
          .then((res) => res.data),
    }),
};
