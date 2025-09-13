import { useCustomMutation } from "@/hooks/useCustomMutation";
import { GoalsApi, UpdateNutritionGoalsRequest } from "@/services/openapi";

import { nutritionGoalsQueryKeys } from "../queries/nutritionGoalsQuery";

const goalsApi = new GoalsApi();

const useUpdateNutritionGoalsMutation = () => {
  const updateNutritionGoalsMutation = useCustomMutation({
    mutationFn: (variables: UpdateNutritionGoalsRequest) =>
      goalsApi.updateNutritionGoals(variables).then((res) => res.data),
    meta: {
      invalidateQueries: [nutritionGoalsQueryKeys.all],
      onSuccessToast: {
        type: "success",
        message: "Nutrition goals have been updated.",
      },
    },
  });

  return updateNutritionGoalsMutation;
};

export default useUpdateNutritionGoalsMutation;
