import { useState } from "react";
import { useQuery } from "@tanstack/react-query";

import { QUERY_KEYS } from "@/features/nutrition/common/data/queryKeys";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import { queryClient } from "@/queryClient";
import { GoalDto, GoalsApi, GoalType } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { retryQueryExcept404 } from "@/services/openapi/retry";

const goalsApi = new GoalsApi();

const useActiveNutritionGoalsQuery = () => {
  const [goalIsNotDefined, setGaolIsNotDefined] = useState(false);

  const activeNutritionGoalsQuery = useQuery({
    queryKey: [QUERY_KEYS.activeNutritionGoals],
    queryFn: () => goalsApi.getActiveNutritionGoals().then((res) => res.data),
    retry: (failureCount, error: ApiError) =>
      retryQueryExcept404(failureCount, error, {
        notFoundCallback: () => setGaolIsNotDefined(true),
      }),
  });

  const isPending = useDelayedLoading(activeNutritionGoalsQuery.isPending);

  let goals;

  if (activeNutritionGoalsQuery.data) {
    goals = {
      calories: activeNutritionGoalsQuery.data.find(
        (goal) => goal.type === GoalType.Calories,
      )!,
      proteins: activeNutritionGoalsQuery.data.find(
        (goal) => goal.type === GoalType.Protein,
      )!,
      carbs: activeNutritionGoalsQuery.data.find(
        (goal) => goal.type === GoalType.Carbohydrates,
      )!,
      fat: activeNutritionGoalsQuery.data.find(
        (goal) => goal.type === GoalType.Fats,
      )!,
    };
  } else {
    goals = undefined;
  }

  return {
    activeNutritionGoalsQuery,
    goalIsNotDefined,
    goals,
    isPending,
  };
};

export const prefetchActiveNutritionGoalsQueryQuery = () => {
  queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.activeNutritionGoals],
    queryFn: () =>
      goalsApi.getActiveGoal(GoalType.Calories).then((res) => res.data),
  });
};

export const invalidateActiveNutritionGoalsQueryQuery = () => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.activeNutritionGoals],
  });
};

export const setActiveNutritionGoalsQueryData = (newValue: number) => {
  queryClient.setQueryData(
    [QUERY_KEYS.activeNutritionGoals],
    (oldData: GoalDto) =>
      ({
        ...oldData,
        newValue,
      }) as GoalDto,
  );
};

export default useActiveNutritionGoalsQuery;
