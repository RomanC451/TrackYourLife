import { ExercisesApi } from "@/services/openapi";

const exercisesApi = new ExercisesApi();

export const exercisesQueryKeys = {
  all: ["exercises"] as const,
  byId: (id: string) => [...exercisesQueryKeys.all, id] as const,
};

export const exercisesQueryOptions = {
  all: {
    queryKey: exercisesQueryKeys.all,
    queryFn: () => exercisesApi.getExercises().then((res) => res.data),
  } as const,
  byId: (id: string) =>
    ({
      queryKey: exercisesQueryKeys.byId(id),
      queryFn: () => exercisesApi.getExerciseById(id).then((res) => res.data),
    }) as const,
};
