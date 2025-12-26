import { useCustomMutation } from "@/hooks/useCustomMutation";
import { CalculateNutritionGoalsRequest, GoalsApi } from "@/services/openapi";

import { nutritionGoalsQueryKeys } from "../queries/nutritionGoalsQuery";

const goalsApi = new GoalsApi();

function useCalculateNutritionGoalsMutation() {
  const calculateNutritionGoalsMutation = useCustomMutation({
    mutationFn: (request: CalculateNutritionGoalsRequest) => {
      return goalsApi
        .calculateNutritionGoals(request)
        .then((resp) => resp.data);
    },
    meta: {
      invalidateQueries: [nutritionGoalsQueryKeys.all],
      awaitInvalidationQuery: nutritionGoalsQueryKeys.all,
      onSuccessToast: {
        type: "success",
        message: "Nutrition goals have been set.",
      },
    },
  });

  return calculateNutritionGoalsMutation;
}

export default useCalculateNutritionGoalsMutation;
