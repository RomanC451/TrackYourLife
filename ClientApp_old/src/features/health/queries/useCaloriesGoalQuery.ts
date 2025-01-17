import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import { ApiError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { queryClient } from "~/queryClient";
import { GoalDto, GoalsApi, GoalType } from "~/services/openapi";
import retryExcept404 from "~/utils/retry";
import { QUERY_KEYS } from "../data/queryKeys";

const goalsApi = new GoalsApi();

const useCaloriesGoalQuery = () => {
  const [goalIsNotDefined, setGaolIsNotDefined] = useState(false);

  const caloriesGoalQuery = useQuery({
    queryKey: [QUERY_KEYS.activeCaloriesGoal],
    queryFn: () =>
      goalsApi.getActiveGoal(GoalType.Calories).then((res) => res.data),
    retry: (failureCount, error: ApiError) =>
      retryExcept404(failureCount, error, {
        notFoundCallback: () => setGaolIsNotDefined(true),
      }),
  });

  const isPending = useDelayedLoading(caloriesGoalQuery.isPending);

  return { caloriesGoalQuery, goalIsNotDefined, isPending };
};

export const prefetchCaloriesGoalQuery = () => {
  queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.activeCaloriesGoal],
    queryFn: () =>
      goalsApi.getActiveGoal(GoalType.Calories).then((res) => res.data),
  });
};

export const invalidateCaloriesGoalQuery = () => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.activeCaloriesGoal],
  });
};

export const setCaloriesGoalQueryData = (newValue: number) => {
  queryClient.setQueryData(
    [QUERY_KEYS.activeCaloriesGoal],
    (oldData: GoalDto) =>
      ({
        ...oldData,
        newValue,
      }) as GoalDto,
  );
};

export default useCaloriesGoalQuery;
