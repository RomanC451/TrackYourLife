import { useState } from "react";
import { StatusCodes } from "http-status-codes";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { getDateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import { GoalDto, GoalPeriod, GoalsApi, GoalType } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { handleApiError } from "@/services/openapi/handleApiError";

import { nutritionGoalsQueryKeys } from "../queries/nutritionGoalsQuery";

const goalsApi = new GoalsApi();

const useUpdateCaloriesGoalMutation = () => {
  const [error, setError] = useState<string | undefined>(undefined);

  const setCaloriesGoalMutation = useCustomMutation({
    mutationFn: (variables: { value: number }) =>
      goalsApi
        .addGoal({
          type: GoalType.Calories,
          value: variables.value,
          period: GoalPeriod.Day,
          startDate: getDateOnly(new Date()),
          force: true,
        })
        .then((res) => res.data),

    meta: {
      invalidateQueries: [nutritionGoalsQueryKeys.all],
      onSuccessToast: {
        type: "success",
        message: "Calories goal has been set.",
      },
    },
    onMutate: () => {
      setError(undefined);
    },
    onSuccess: (_, variables) => {
      queryClient.setQueryData(
        nutritionGoalsQueryKeys.all,
        (oldData: GoalDto[]) => [
          ...oldData,
          {
            type: GoalType.Calories,
            value: variables.value,
            period: GoalPeriod.Day,
          },
        ],
      );
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

  return { setCaloriesGoalMutation, error };
};

export default useUpdateCaloriesGoalMutation;
