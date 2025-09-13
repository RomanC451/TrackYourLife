import { useState } from "react";
import { StatusCodes } from "http-status-codes";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { getDateOnly } from "@/lib/date";
import { GoalPeriod, GoalsApi, GoalType } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { nutritionGoalsQueryKeys } from "../queries/nutritionGoalsQuery";

const goalsApi = new GoalsApi();

const useUpdateGoalMutation = () => {
  const [error, setError] = useState<string | undefined>(undefined);

  const updateGoalMutation = useCustomMutation({
    mutationFn: (variables: { id: string; type: GoalType; value: number }) =>
      goalsApi
        .updateGoal({
          id: variables.id,
          type: variables.type,
          value: variables.value,
          period: GoalPeriod.Day,
          startDate: getDateOnly(new Date()),
        })
        .then((res) => res.data),
    meta: {
      invalidateQueries: [nutritionGoalsQueryKeys.all],
      onSuccessToast: {
        type: "success",
        message: "Goal has been updated.",
      },
    },
    onMutate: () => {
      setError(undefined);
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

  return { updateGoalMutation, error };
};

export default useUpdateGoalMutation;
