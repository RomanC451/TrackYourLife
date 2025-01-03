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
import { setNutritionDiariesQueryData } from "../queries/useNutritionDiariesQuery";
import { setTotalCaloriesQueryData } from "../queries/useTotalCaloriesQuery";
import foodDiaryAddedToast from "../toasts/foodDiaryAddedToast";

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

      setTotalCaloriesQueryData({
        adjustment: food.nutritionalContents.energy.value * variables.quantity,
        invalidate: true,
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
