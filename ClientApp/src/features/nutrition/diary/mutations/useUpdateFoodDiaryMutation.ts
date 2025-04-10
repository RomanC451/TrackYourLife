import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DateOnly } from "@/lib/date";
import {
  FoodDiariesApi,
  ServingSizeDto,
  UpdateFoodDiaryRequest,
} from "@/services/openapi";

import { invalidateDailyNutritionOverviewsQuery } from "../../overview/queries/useDailyNutritionOverviewsQuery";
import { setNutritionDiariesQueryData } from "../queries/useNutritionDiariesQuery";
import { invalidateNutritionOverviewQuery } from "../queries/useNutritionOverviewQuery";

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
      invalidateDailyNutritionOverviewsQuery();

      invalidateDailyNutritionOverviewsQuery()
      invalidateNutritionOverviewQuery(variables.date, variables.date);

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
