import { queryOptions } from "@tanstack/react-query";

import { TrainingsApi } from "@/services/openapi";

const trainingsApi = new TrainingsApi();

export const workoutStreakQueryKeys = {
  all: ["workoutStreak"] as const,
};

export const workoutStreakQueryOptions = {
  current: queryOptions({
    queryKey: workoutStreakQueryKeys.all,
    queryFn: () => trainingsApi.getWorkoutStreak().then((res) => res.data),
  }),
};
