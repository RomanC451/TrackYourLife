import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { GoalsApi, UpdateNutritionGoalsRequest } from "@/services/openapi";

import { invalidateNutritionGoalsQueryQuery } from "../queries/useNutritionGoalQueries";

const goalsApi = new GoalsApi();

const useUpdateNutritionGoalsMutation = () => {
  const updateNutritionGoalsMutation = useMutation({
    mutationFn: (variables: UpdateNutritionGoalsRequest) =>
      goalsApi.updateNutritionGoals(variables).then((res) => res.data),
    onSuccess: () => {
      toast.success("Nutrition goals have been updated.");
      invalidateNutritionGoalsQueryQuery();
    },
  });

  const isPending = useDelayedLoading(updateNutritionGoalsMutation.isPending);

  return { updateNutritionGoalsMutation, isPending };
};

export default useUpdateNutritionGoalsMutation;
