import { ErrorOption } from "react-hook-form";
import { v4 as uuidv4 } from "uuid";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import {
  CreateWorkoutPlanRequest,
  TrainingDto,
  WorkoutPlanDto,
  WorkoutPlansApi,
} from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { trainingsQueryKeys } from "../../trainings/queries/trainingsQueries";
import {
  workoutPlansQueryKeys,
  workoutPlansQueryOptions,
} from "../queries/workoutPlansQueries";

const workoutPlansApi = new WorkoutPlansApi();

export type WorkoutPlanCreateMutationVariables = {
  request: CreateWorkoutPlanRequest;
  setError?: (
    name: "name" | "trainingIds",
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
};

const useCreateWorkoutPlanMutation = () => {
  return useCustomMutation({
    mutationFn: ({ request }: WorkoutPlanCreateMutationVariables) =>
      workoutPlansApi.createWorkoutPlan(request),
    meta: {
      onSuccessToast: {
        message: "Workout plan created",
        type: "success",
      },
      invalidateQueries: [
        workoutPlansQueryKeys.all,
        workoutPlansQueryKeys.active,
        workoutPlansQueryKeys.nextWorkout,
      ],
    },
    onSuccess: async (_, { request }) => {
      const cachedTrainings = queryClient.getQueryData<TrainingDto[]>(
        trainingsQueryKeys.all,
      );

      const workouts =
        cachedTrainings
          ?.filter((training) => request.trainingIds.includes(training.id))
          .sort(
            (a, b) =>
              request.trainingIds.indexOf(a.id) -
              request.trainingIds.indexOf(b.id),
          ) ?? [];

      const optimisticPlan = {
        id: uuidv4(),
        name: request.name,
        isActive: request.isActive,
        workouts,
        createdOnUtc: new Date().toISOString(),
        modifiedOnUtc: undefined,
        isDeleting: false,
        isLoading: true,
      } as unknown as WorkoutPlanDto;

      queryClient.setQueryData(
        workoutPlansQueryKeys.all,
        (oldData: WorkoutPlanDto[] = []) =>
          [...oldData, optimisticPlan].sort((a, b) =>
            a.name.localeCompare(b.name),
          ),
      );

      if (request.isActive) {
        queryClient.setQueryData(workoutPlansQueryKeys.active, optimisticPlan);
      }

      await queryClient.invalidateQueries({
        queryKey: workoutPlansQueryOptions.all.queryKey,
      });
    },
    onError: (error, { setError }) => {
      handleApiError({
        error,
        validationErrorsHandler: setError,
      });
    },
  });
};

export default useCreateWorkoutPlanMutation;
