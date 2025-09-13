import { useCustomMutation } from "@/hooks/useCustomMutation";
import { DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import {
  FoodDiariesApi,
  FoodDto,
  UpdateFoodDiaryRequest,
} from "@/services/openapi";

import { multiplyNutritionalContent } from "../../common/utils/nutritionalContent";
import { dailyNutritionOverviewsQueryKeys } from "../../overview/queries/useDailyNutritionOverviewsQuery";
import {
  foodDiariesQueryKeys,
  nutritionDiariesQueryKeys,
  setNutritionDiariesQueryData,
} from "../queries/useDiaryQuery";

const foodDiariesApi = new FoodDiariesApi();

export type UpdateFoodDiaryMutationVariables = UpdateFoodDiaryRequest & {
  id: string;
};

const useUpdateFoodDiaryMutation = (food: FoodDto) => {
  const updateFoodDiaryMutation = useCustomMutation({
    mutationFn: (variables: UpdateFoodDiaryMutationVariables) => {
      const { id, ...request } = variables;

      return foodDiariesApi.updateFoodDiary(id, request);
    },
    meta: {
      invalidateQueries: [dailyNutritionOverviewsQueryKeys.all],
      onSuccessToast: {
        type: "success",
        message: "Food diary updated successfully",
      },
    },
    onSuccess: (_, variables) => {
      const servingSize = Object.values(food.servingSizes).find(
        (ss) => ss.id == variables.servingSizeId,
      )!;

      queryClient.invalidateQueries({
        queryKey: dailyNutritionOverviewsQueryKeys.all,
      });

      queryClient.invalidateQueries({
        queryKey: nutritionDiariesQueryKeys.byDate(
          variables.entryDate as DateOnly,
        ),
      });

      queryClient.invalidateQueries({
        queryKey: nutritionDiariesQueryKeys.overview(
          variables.entryDate as DateOnly,
          variables.entryDate as DateOnly,
        ),
      });

      queryClient.invalidateQueries({
        queryKey: foodDiariesQueryKeys.byId(variables.id),
      });

      setNutritionDiariesQueryData({
        date: variables.entryDate as DateOnly,
        mealType: variables.mealType,
        updatedDiary: {
          id: variables.id,
          quantity: variables.quantity,
          servingSize: servingSize,
          mealType: variables.mealType,
          nutritionalContents: multiplyNutritionalContent(
            food.nutritionalContents,
            variables.quantity * servingSize.nutritionMultiplier,
          ),
        },
      });

      // queryClient.invalidateQueries({
      //   queryKey: nutritionDiariesQueryKeys.all,
      // });
    },
  });

  return updateFoodDiaryMutation;
};

export default useUpdateFoodDiaryMutation;
