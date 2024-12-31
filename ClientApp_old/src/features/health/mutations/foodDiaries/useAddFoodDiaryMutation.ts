import { DateOnly } from "~/utils/date";

import { useMutation } from "@tanstack/react-query";
import { toastDefaultServerError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import {
  AddFoodDiaryRequest,
  DiaryType,
  FoodDiariesApi,
  FoodDto,
  ServingSizeDto,
} from "~/services/openapi/api";
import { setNutritionDiariesQueryData } from "../../queries/foodDiaries/useNutritionDiariesQuery";
import { setTotalCaloriesQueryData } from "../../queries/useTotalCaloriesQuery";
import foodDiaryAddedToast from "../../toasts/foodDiaries/foodDiaryAddedToast";
import { multiplyNutritionalContent } from "../../utils/nutritionalContent";

const foodDiariesApi = new FoodDiariesApi();

export interface AddFoodDiaryMutationVariables extends AddFoodDiaryRequest {
  food: FoodDto;
  servingSize: ServingSizeDto;
}

const useAddFoodDiaryMutation = () => {
  const addFoodDiaryMutation = useMutation({
    mutationFn: (variables: AddFoodDiaryMutationVariables) => {
      const { food, servingSize, ...addFoodDiaryRequest } = variables;
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
