import { keepPreviousData, queryOptions } from "@tanstack/react-query";

import { ExercisesApi } from "@/services/openapi";

const exercisesApi = new ExercisesApi();

export const topExercisesQueryKeys = {
  all: ["topExercises"] as const,
  byPage: (page: number, pageSize: number) =>
    [...topExercisesQueryKeys.all, page, pageSize] as const,
};

export const topExercisesQueryOptions = {
  byPage: (page: number, pageSize: number) =>
    queryOptions({
      queryKey: topExercisesQueryKeys.byPage(page, pageSize),
      queryFn: () =>
        exercisesApi.getTopExercises(page, pageSize).then((res) => res.data),
      placeholderData: keepPreviousData,
    }),
};