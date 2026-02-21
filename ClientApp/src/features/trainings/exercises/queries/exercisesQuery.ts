import { queryOptions } from "@tanstack/react-query";

import { ExercisesApi } from "@/services/openapi";

const exercisesApi = new ExercisesApi();

export const exercisesQueryKeys = {
  all: ["exercises"] as const,
  byId: (id: string) => [...exercisesQueryKeys.all, id] as const,
};

export const exercisesQueryOptions = {
  all: queryOptions({
    queryKey: exercisesQueryKeys.all,
    queryFn: () => exercisesApi.getExercises().then((res) => res.data),
  }),
  byId: (id: string) =>
    queryOptions({
      queryKey: exercisesQueryKeys.byId(id),
      queryFn: () => exercisesApi.getExerciseById(id).then((res) => res.data),
    }),
};
