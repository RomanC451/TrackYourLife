import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";
import { toast } from "sonner";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { getDateOnly } from "@/lib/date";
import { GoalPeriod, GoalsApi, GoalType } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";

import {
  invalidateNutritionGoalsQueryQuery,
  setNutritionGoalsQueryData,
} from "../queries/useNutritionGoalQueries";
import { handleApiError } from "@/services/openapi/handleApiError";

const goalsApi = new GoalsApi();

const useSetCaloriesGoalMutation = () => {
  const [error, setError] = useState<string | undefined>(undefined);

  const setCaloriesGoalMutation = useMutation({
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
    onMutate: () => {
      setError(undefined);
    },
    onSuccess: (_, variables) => {
      toast.success("Calories goal has been set.");

      invalidateNutritionGoalsQueryQuery();

      setNutritionGoalsQueryData(variables.value);
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

  const isPending = useDelayedLoading(setCaloriesGoalMutation.isPending);

  return { setCaloriesGoalMutation, isPending, error };
};

export default useSetCaloriesGoalMutation;
