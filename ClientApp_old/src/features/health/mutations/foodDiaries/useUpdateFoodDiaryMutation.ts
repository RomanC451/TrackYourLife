import { useMutation } from "@tanstack/react-query";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import {
  FoodDiariesApi,
  ServingSizeDto,
  UpdateFoodDiaryRequest,
} from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import {
  invalidateNutritionDiariesQuery,
  setNutritionDiariesQueryData,
} from "../../queries/foodDiaries/useNutritionDiariesQuery";

const foodDiariesApi = new FoodDiariesApi();

interface UpdateFoodDiaryMutationVariables extends UpdateFoodDiaryRequest {
  date: DateOnly;
  servingSize: ServingSizeDto;
}

const useUpdateFoodDiaryMutation = () => {
  const updateFoodDiaryMutation = useMutation({
    mutationFn: (variables: UpdateFoodDiaryMutationVariables) =>
      foodDiariesApi.updateFoodDiary(variables),
    onSuccess: (_, variables) => {
      invalidateNutritionDiariesQuery();

      setNutritionDiariesQueryData({
        date: variables.date,
        mealType: variables.mealType,
        updatedDiary: {
          id: variables.id,
          quantity: variables.quantity,
          servingSize: variables.servingSize,
          mealType: variables.mealType,
        },
        invalidate: true,
      });
    },
  });

  const isPending = useDelayedLoading(updateFoodDiaryMutation.isPending);

  return { updateFoodDiaryMutation, isPending };
};

export default useUpdateFoodDiaryMutation;
