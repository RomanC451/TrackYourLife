import { ErrorOption } from "react-hook-form";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { UpdateWorkoutPlanRequest, WorkoutPlansApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { workoutPlansQueryKeys } from "../queries/workoutPlansQueries";

const workoutPlansApi = new WorkoutPlansApi();

export type WorkoutPlanUpdateMutationVariables = {
  id: string;
  request: UpdateWorkoutPlanRequest;
  setError?: (
    name: "name" | "trainingIds",
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
};

const useUpdateWorkoutPlanMutation = () => {
  return useCustomMutation({
    mutationFn: ({ id, request }: WorkoutPlanUpdateMutationVariables) =>
      workoutPlansApi.updateWorkoutPlan(id, request),
    meta: {
      onSuccessToast: {
        message: "Workout plan updated",
        type: "success",
      },
      invalidateQueries: [
        workoutPlansQueryKeys.all,
        workoutPlansQueryKeys.active,
        workoutPlansQueryKeys.nextWorkout,
      ],
    },
    onError: (error: ApiError, { setError }) => {
      handleApiError({
        error,
        validationErrorsHandler: setError,
      });
    },
  });
};

export default useUpdateWorkoutPlanMutation;
