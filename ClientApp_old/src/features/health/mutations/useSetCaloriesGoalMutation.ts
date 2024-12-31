import { useMutation } from "@tanstack/react-query";
import { StatusCodes } from "http-status-codes";
import { useState } from "react";
import { toast } from "sonner";
import { ApiError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { GoalPeriod, GoalsApi, GoalType } from "~/services/openapi";
import { getDateOnly } from "~/utils/date";
import { handleApiError } from "~/utils/handleApiError";
import {
  invalidateCaloriesGoalQuery,
  setCaloriesGoalQueryData,
} from "../queries/useCaloriesGoalQuery";

const goalsApi = new GoalsApi();

const useSetCaloriesGoalMutation = () => {
  const [error, setError] = useState<string | undefined>(undefined);

  const setCaloriesGoalMutation = useMutation({
    mutationFn: (variables: { value: number }) =>
      goalsApi
        .addGoal({
          type: GoalType.Calories,
          value: variables.value,
          perPeriod: GoalPeriod.Day,
          startDate: getDateOnly(new Date()),
          force: true,
        })
        .then((res) => res.data),
    onMutate: () => {
      setError(undefined);
    },
    onSuccess: (_, variables) => {
      toast.success("Calories goal has been set.");

      invalidateCaloriesGoalQuery();

      setCaloriesGoalQueryData(variables.value);
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
