import { queryOptions } from "@tanstack/react-query";

import { ExercisesHistoriesApi } from "@/services/openapi/api";

const exerciseHistoryApi = new ExercisesHistoriesApi();

export const exerciseHistoryQueryKeys = {
  all: ["exerciseHistory"] as const,
  byExerciseId: (exerciseId: string) =>
    [...exerciseHistoryQueryKeys.all, exerciseId] as const,
};

export const exerciseHistoryQueryOptions = {
  byExerciseId: (exerciseId: string) =>
    queryOptions({
      queryKey: exerciseHistoryQueryKeys.byExerciseId(exerciseId),
      queryFn: () =>
        exerciseHistoryApi
          .getExerciseHistoryByExerciseId(exerciseId)
          .then((res) => res.data),
    }),
};
