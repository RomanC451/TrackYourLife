import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { ExercisesHistoriesApi } from "@/services/openapi";

const exercisesHistoriesApi = new ExercisesHistoriesApi();

export type PerformanceCalculationMethod = "Sequential" | "FirstVsLast";

export const exercisePerformanceQueryKeys = {
  all: ["exercisePerformance"] as const,
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    exerciseId: string | null,
    calculationMethod: PerformanceCalculationMethod,
    page: number,
    pageSize: number,
  ) =>
    [
      ...exercisePerformanceQueryKeys.all,
      startDate,
      endDate,
      exerciseId,
      calculationMethod,
      page,
      pageSize,
    ] as const,
};

export const exercisePerformanceQueryOptions = {
  byFilters: (
    startDate: DateOnly | null,
    endDate: DateOnly | null,
    exerciseId: string | null,
    calculationMethod: PerformanceCalculationMethod = "Sequential",
    page: number = 1,
    pageSize: number = 10,
  ) =>
    queryOptions({
      queryKey: exercisePerformanceQueryKeys.byFilters(
        startDate,
        endDate,
        exerciseId,
        calculationMethod,
        page,
        pageSize,
      ),
      queryFn: () =>
        exercisesHistoriesApi
          .getExercisePerformance(
            page,
            pageSize,
            startDate,
            endDate ,
            exerciseId ,
            calculationMethod,
          )
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
