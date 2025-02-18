import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { CalculateNutritionGoalsRequest, GoalsApi } from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

const goalsApi = new GoalsApi();

function useCalculateNutritionGoalsMutation() {
  const calculateNutritionGoalsMutation = useMutation({
    mutationFn: (request: CalculateNutritionGoalsRequest) => {
      return goalsApi
        .calculateNutritionGoals(request)
        .then((resp) => resp.data);
    },
    onError: (error) => {
      toastDefaultServerError(error);
    },
    onSuccess: () => {},
  });

  const isPending = useDelayedLoading(
    calculateNutritionGoalsMutation.isPending,
  );

  return { calculateNutritionGoalsMutation, isPending };
}

export default useCalculateNutritionGoalsMutation;
