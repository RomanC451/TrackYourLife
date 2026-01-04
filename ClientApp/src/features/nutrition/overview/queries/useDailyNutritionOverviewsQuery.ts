import { queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import {
  AggregationMode,
  DailyNutritionOverviewDto,
  DailyNutritionOverviewsApi,
  OverviewType,
} from "@/services/openapi";

import { createEmptyNutritionalContent } from "../../common/utils/nutritionalContent";

const dailyNutritionOverviewsApi = new DailyNutritionOverviewsApi();

export const dailyNutritionOverviewsQueryKeys = {
  all: ["dailyNutritionOverviews"] as const,
  byDateRange: (
    startDate: DateOnly,
    endDate: DateOnly,
    overviewType: OverviewType,
    aggregationMode: AggregationMode,
  ) =>
    [
      ...dailyNutritionOverviewsQueryKeys.all,
      startDate,
      endDate,
      overviewType,
      aggregationMode,
    ] as const,
};

export const dailyNutritionOverviewsQueryOptions = {
  byDateRange: (
    startDate: DateOnly,
    endDate: DateOnly,
    overviewType: OverviewType,
    aggregationMode: AggregationMode,
  ) =>
    queryOptions({
      queryKey: dailyNutritionOverviewsQueryKeys.byDateRange(
        startDate,
        endDate,
        overviewType,
        aggregationMode,
      ),
      queryFn: () =>
        dailyNutritionOverviewsApi
          .getDailyNutritionOverviewsByDateRange(
            startDate,
            endDate,
            overviewType,
            aggregationMode,
          )
          .then((res) => res.data),

      placeholderData: (prev) =>
        prev ?? [
          {
            date: startDate,
            nutritionalContent: createEmptyNutritionalContent(),
            caloriesGoal: 0,
            carbohydratesGoal: 0,
            proteinGoal: 0,
            fatGoal: 0,
            isLoading: false,
            isDeleting: false,
            id: "",
            startDate: startDate,
            endDate: endDate,
          } as DailyNutritionOverviewDto,
        ],
    }),
};
