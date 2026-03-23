import { queryOptions } from "@tanstack/react-query";

import { queryClient } from "@/queryClient";
import {
  TrainingDto,
  WorkoutPlanDto,
  WorkoutPlansApi,
} from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { retryQueryExcept404 } from "@/services/openapi/retry";

const workoutPlansApi = new WorkoutPlansApi();

export const workoutPlansQueryKeys = {
  all: ["workoutPlans"] as const,
  active: [...["workoutPlans"], "active"] as const,
  nextWorkout: [...["workoutPlans"], "nextWorkout"] as const,
  byId: (id: string) => [...workoutPlansQueryKeys.all, id] as const,
};

export const workoutPlansQueryOptions = {
  all: {
    queryKey: workoutPlansQueryKeys.all,
    queryFn: () => workoutPlansApi.getWorkoutPlans().then((res) => res.data),
  } as const,
  active: queryOptions({
    queryKey: workoutPlansQueryKeys.active,
    queryFn: () =>
      workoutPlansApi
        .getActiveWorkoutPlan()
        .then<WorkoutPlanDto | null>((res) => res.data),
    retry: (failureCount: number, error: ApiError) =>
      retryQueryExcept404(failureCount, error, {
        notFoundCallback: (error) => {
          if (
            error.response?.data?.type === "WorkoutPlans.ActivePlanNotFound"
          ) {
            queryClient.setQueryData(workoutPlansQueryKeys.active, null);
          }
        },
      }),
  }),
  nextWorkout: {
    queryKey: workoutPlansQueryKeys.nextWorkout,
    queryFn: () =>
      workoutPlansApi.getNextWorkoutFromActivePlan().then((res) => res.data),
    retry: (failureCount: number, error: ApiError) =>
      retryQueryExcept404(failureCount, error, {
        notFoundCallback: (error) => {
          if (
            error.response?.data?.type === "WorkoutPlans.ActivePlanNotFound"
          ) {
            queryClient.setQueryData(workoutPlansQueryKeys.nextWorkout, null);
          }
        },
      }),
  } as const,
  byId: (id: string) =>
    ({
      queryKey: workoutPlansQueryKeys.byId(id),
      queryFn: () => workoutPlansApi.getWorkoutPlans().then((res) => res.data),
      select: (data: WorkoutPlanDto[]) =>
        data.find((workoutPlan) => workoutPlan.id === id)!,
    }) as const,
  containsTrainingId: (trainingId: string) =>
    ({
      queryKey: [...workoutPlansQueryKeys.all, "contains", trainingId],
      queryFn: () => workoutPlansApi.getWorkoutPlans().then((res) => res.data),
      select: (data: WorkoutPlanDto[]) =>
        data.filter((workoutPlan) =>
          workoutPlan.workouts.some(
            (workout: TrainingDto) => workout.id === trainingId,
          ),
        ),
    }) as const,
};
