import { useQuery } from "@tanstack/react-query";

import { QUERY_KEYS } from "@/features/nutrition/common/data/queryKeys";
import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DateOnly, getDateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import { GoalDto, GoalsApi, GoalType } from "@/services/openapi";
import { ApiError } from "@/services/openapi/apiSettings";
import { retryQueryExcept404 } from "@/services/openapi/retry";
import { StatusCodes } from "http-status-codes";

const goalsApi = new GoalsApi();

const useNutritionGoalsQuery = (date: DateOnly) => {

  const nutritionGoalsQuery = useQuery({
    queryKey: [QUERY_KEYS.nutritionGoals],
    queryFn: () => goalsApi.getNutritionGoals(date).then((res) => res.data),
    retry: (failureCount, error: ApiError) =>
      retryQueryExcept404(failureCount, error, {
      }),
    staleTime: Infinity,
    placeholderData: (prev) => prev,
  });


  const isPending = useDelayedLoading(nutritionGoalsQuery.isLoading);

  let goals;

  if (nutritionGoalsQuery.data && nutritionGoalsQuery.data.length > 0) {
    goals = {
      calories: nutritionGoalsQuery.data.find(
        (goal) => goal.type === GoalType.Calories,
      )!,
      proteins: nutritionGoalsQuery.data.find(
        (goal) => goal.type === GoalType.Protein,
      )!,
      carbs: nutritionGoalsQuery.data.find(
        (goal) => goal.type === GoalType.Carbohydrates,
      )!,
      fat: nutritionGoalsQuery.data.find(
        (goal) => goal.type === GoalType.Fats,
      )!,
    };
  } else {
    goals = undefined;
  }

  return {
    goalsAreNotDefined: nutritionGoalsQuery.error?.status === StatusCodes.NOT_FOUND || nutritionGoalsQuery.data?.length === 0,
    goals,
    isPending,
  };
};

export const prefetchNutritionGoalQueryQuery = (date?: DateOnly) => {
  const d = date ?? getDateOnly(new Date());

  return queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.nutritionGoals],
    queryFn: () => goalsApi.getNutritionGoals(d).then((res) => res.data),
    retry: false,
  });
};

export const invalidateNutritionGoalsQueryQuery = () => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.nutritionGoals],
  });
};

export const setNutritionGoalsQueryData = (newValue: number) => {
  queryClient.setQueryData(
    [QUERY_KEYS.nutritionGoals],
    (oldData: GoalDto) =>
      ({
        ...oldData,
        newValue,
      }) as GoalDto,
  );
};

export default useNutritionGoalsQuery;
