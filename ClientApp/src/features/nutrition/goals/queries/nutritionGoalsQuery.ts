import { queryOptions } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";
import { GoalsApi, GoalType } from "@/services/openapi";
import { retryQueryExcept404 } from "@/services/openapi/retry";

const goalsApi = new GoalsApi();

export const nutritionGoalsQueryKeys = {
  all: ["nutritionGoals"] as const,
  byDate: (date: DateOnly) => [...nutritionGoalsQueryKeys.all, date] as const,
};

export const nutritionGoalsQueryOptions = {
  byDate: (date: DateOnly) =>
    queryOptions({
      queryKey: nutritionGoalsQueryKeys.byDate(date),
      queryFn: () => goalsApi.getNutritionGoals(date).then((res) => res.data),
      retry: (failureCount, error) =>
        retryQueryExcept404(failureCount, error, {}),
      placeholderData: (prev) => prev,
      select: (data) => ({
        calories: data.find((goal) => goal.type === GoalType.Calories)!,
        proteins: data.find((goal) => goal.type === GoalType.Protein)!,
        carbs: data.find((goal) => goal.type === GoalType.Carbohydrates)!,
        fat: data.find((goal) => goal.type === GoalType.Fats)!,
      }),
    }),
};
