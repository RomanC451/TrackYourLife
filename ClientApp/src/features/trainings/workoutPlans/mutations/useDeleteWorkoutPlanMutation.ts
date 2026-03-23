import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { WorkoutPlanDto, WorkoutPlansApi } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { workoutPlansQueryKeys } from "../queries/workoutPlansQueries";

const workoutPlansApi = new WorkoutPlansApi();

type DeleteWorkoutPlanMutationVariables = {
  id: string;
};

const useDeleteWorkoutPlanMutation = () => {
  return useCustomMutation({
    mutationFn: ({ id }: DeleteWorkoutPlanMutationVariables) =>
      workoutPlansApi.deleteWorkoutPlan(id),
    meta: {
      onSuccessToast: {
        message: "Workout plan deleted",
        type: "success",
      },
      invalidateQueries: [
        workoutPlansQueryKeys.all,
        workoutPlansQueryKeys.active,
        workoutPlansQueryKeys.nextWorkout,
      ],
    },
    onSuccess: (_, { id }) => {
      queryClient.setQueryData(
        workoutPlansQueryKeys.all,
        (oldData: WorkoutPlanDto[] = []) => oldData.filter((plan) => plan.id !== id),
      );
    },
    onError: (error: ApiError) => {
      handleApiError({ error });
    },
  });
};

export default useDeleteWorkoutPlanMutation;
