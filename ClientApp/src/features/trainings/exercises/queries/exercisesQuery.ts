import { queryOptions } from "@tanstack/react-query";

import { queryClient } from "@/queryClient";
import { ExercisesApi } from "@/services/openapi";
import { preloadImage } from "@/services/openapi/preload";

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

/** For route loaders / preload: cache exercise and warm picture when present. */
export async function ensureExerciseByIdWithPicturePreload(exerciseId: string) {
  const exercise = await queryClient.ensureQueryData(
    exercisesQueryOptions.byId(exerciseId),
  );
  if (exercise.pictureUrl) {
    preloadImage(exercise.pictureUrl);
  }
  return exercise;
}

/** For create-exercise dialog routes. */
export async function ensureExercisesList() {
  await queryClient.ensureQueryData(exercisesQueryOptions.all);
}
