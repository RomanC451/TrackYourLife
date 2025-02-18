import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";
import { toast } from "sonner";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { handleApiError } from "@/lib/handleApiError";
import { GoalPeriod, GoalsApi, GoalType } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import { invalidateActiveNutritionGoalsQueryQuery } from "../queries/useCaloriesGoalQuery";
import { getDateOnly } from "@/lib/date";

const goalsApi = new GoalsApi();

const useUpdateGoalMutation = () => {
  const [error, setError] = useState<string | undefined>(undefined);

  const updateGoalMutation = useMutation({
    mutationFn: (variables: { id: string; type: GoalType; value: number }) =>
      goalsApi
        .updateGoal({
            id: variables.id,
          type: variables.type,
          value: variables.value,
          perPeriod: GoalPeriod.Day,
          startDate: getDateOnly(new Date()),
        })
        .then((res) => res.data),
    onMutate: () => {
      setError(undefined);
    },
    onSuccess: () => {
      toast.success("Goal has been updated.");
      invalidateActiveNutritionGoalsQueryQuery();
    },
    onError: (error: ApiError) =>
      handleApiError({
        error,
        errorHandlers: {
          [StatusCodes.BAD_REQUEST]: {
            ValidationError: () => setError("Value must be greater than 0."),
          },
        },
      }),
  });

  const isPending = useDelayedLoading(updateGoalMutation.isPending);

  return { updateGoalMutation, isPending, error };
};

export default useUpdateGoalMutation; 