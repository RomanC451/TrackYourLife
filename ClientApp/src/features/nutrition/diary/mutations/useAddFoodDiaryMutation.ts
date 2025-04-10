import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DateOnly } from "@/lib/date";
import {
  AddFoodDiaryRequest,
  DiaryType,
  FoodDiariesApi,
  FoodDto,
  ServingSizeDto,
} from "@/services/openapi/api";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { multiplyNutritionalContent } from "../../common/utils/nutritionalContent";
import { invalidateDailyNutritionOverviewsQuery } from "../../overview/queries/useDailyNutritionOverviewsQuery";
import { setNutritionDiariesQueryData } from "../queries/useNutritionDiariesQuery";
import { setNutritionOverviewQueryData } from "../queries/useNutritionOverviewQuery";
import foodDiaryAddedToast from "../toasts/foodDiaryAddedToast";
import { invalidateFoodSearchQuery } from "../../common/queries/useFoodSearchQuery";

const foodDiariesApi = new FoodDiariesApi();

export interface AddFoodDiaryMutationVariables extends AddFoodDiaryRequest {
  food: FoodDto;
  servingSize: ServingSizeDto;
}

const useAddFoodDiaryMutation = () => {
  const addFoodDiaryMutation = useMutation({
    mutationFn: (variables: AddFoodDiaryMutationVariables) => {
      const { ...addFoodDiaryRequest } = variables;
      return foodDiariesApi
        .addFoodDiary(addFoodDiaryRequest)
        .then((resp) => resp.data);
    },

    onSuccess: (resp, variables) => {
      const { food } = variables;

      const servingSize = Object.values(food.servingSizes).find(
        (ss) => ss.id == variables.servingSizeId,
      )!;

      foodDiaryAddedToast({
        food: food,
        mealType: variables.mealType,
        quantity: variables.quantity,
        servingSize: servingSize,
      });

      invalidateDailyNutritionOverviewsQuery()
      invalidateFoodSearchQuery()

      setNutritionOverviewQueryData({
        adjustment: multiplyNutritionalContent(
          food.nutritionalContents,
          variables.quantity * servingSize.nutritionMultiplier,
        ),
        invalidate: true,
        startDate: variables.entryDate as DateOnly,
        endDate: variables.entryDate as DateOnly,
      });

      setNutritionDiariesQueryData({
        date: variables.entryDate as DateOnly,
        mealType: variables.mealType,
        newDiary: {
          id: resp.id,
          name: food.name,
          nutritionalContents: multiplyNutritionalContent(
            food.nutritionalContents,
            servingSize.nutritionMultiplier,
          ),
          nutritionMultiplier: servingSize.nutritionMultiplier,
          servingSize: servingSize,
          diaryType: DiaryType.FoodDiary,
          quantity: variables.quantity,
          mealType: variables.mealType,
          date: variables.entryDate,
        },
        invalidate: true,
      });
    },
    onError: toastDefaultServerError,
  });

  const isPending = useDelayedLoading(addFoodDiaryMutation.isPending);

  return { addFoodDiaryMutation, isPending };
};

export default useAddFoodDiaryMutation;
