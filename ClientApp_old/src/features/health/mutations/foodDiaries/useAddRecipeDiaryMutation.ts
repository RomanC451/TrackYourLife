import { useMutation } from "@tanstack/react-query";
import { toastDefaultServerError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import {
  AddRecipeDiaryRequest,
  DiaryType,
  RecipeDiariesApi,
  RecipeDto,
} from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import { setNutritionDiariesQueryData } from "../../queries/foodDiaries/useNutritionDiariesQuery";
import { setTotalCaloriesQueryData } from "../../queries/useTotalCaloriesQuery";
import recipeDiaryAddedToast from "../../toasts/foodDiaries/recipeDiaryAddedToast";
import { multiplyNutritionalContent } from "../../utils/nutritionalContent";

const recipeDiariesApi = new RecipeDiariesApi();

export interface AddRecipeDiaryMutationVariables extends AddRecipeDiaryRequest {
  recipe: RecipeDto;
}

export default function useAddRecipeDiaryMutation() {
  const addRecipeDiaryMutation = useMutation({
    mutationFn: (variables: AddRecipeDiaryMutationVariables) => {
      const { recipe, ...addRecipeDiaryRequest } = variables;
      return recipeDiariesApi
        .addRecipeDiary(addRecipeDiaryRequest)
        .then((resp) => resp.data);
    },
    onSuccess: (resp, variables) => {
      const { recipe } = variables;

      recipeDiaryAddedToast({
        name: recipe.name,
        mealType: variables.mealType,
        numberOfServings: variables.quantity,
      });

      setTotalCaloriesQueryData({
        adjustment:
          recipe.nutritionalContents.energy.value * variables.quantity,
        invalidate: true,
      });

      setNutritionDiariesQueryData({
        date: variables.entryDate as DateOnly,
        mealType: variables.mealType,
        newDiary: {
          id: resp.id,
          name: recipe.name,
          nutritionalContents: multiplyNutritionalContent(
            recipe.nutritionalContents,
            variables.quantity,
          ),
          nutritionMultiplier: variables.quantity,
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

  const isPending = useDelayedLoading(addRecipeDiaryMutation.isPending);

  return { addRecipeDiaryMutation, isPending };
}
