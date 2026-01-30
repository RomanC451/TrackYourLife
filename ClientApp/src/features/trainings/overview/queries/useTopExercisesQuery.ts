import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { ExercisesApi } from "@/services/openapi";

const exercisesApi = new ExercisesApi();

export const topExercisesQueryKeys = {
  all: ["topExercises"] as const,
  byPage: (page: number, pageSize: number) =>
    [...topExercisesQueryKeys.all, page, pageSize] as const,
  byFilters: (
    page: number,
    pageSize: number,
    startDate: DateOnly | null,
    endDate: DateOnly | null,
  ) =>
    [...topExercisesQueryKeys.all, page, pageSize, startDate, endDate] as const,
};

export const topExercisesQueryOptions = {
  byPage: (
    page: number,
    pageSize: number,
    startDate: DateOnly | null = null,
    endDate: DateOnly | null = null,
  ) =>
    queryOptions({
      queryKey: topExercisesQueryKeys.byFilters(
        page,
        pageSize,
        startDate,
        endDate,
      ),
      queryFn: () =>
        exercisesApi
          .getTopExercises(page, pageSize, startDate, endDate)
          .then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};
