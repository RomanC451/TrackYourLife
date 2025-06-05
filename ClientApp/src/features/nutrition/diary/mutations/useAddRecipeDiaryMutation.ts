import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DateOnly } from "@/lib/date";
import {
  AddRecipeDiaryRequest,
  DiaryType,
  RecipeDiariesApi,
  RecipeDto,
} from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { multiplyNutritionalContent } from "../../common/utils/nutritionalContent";
import { setNutritionDiariesQueryData } from "../queries/useNutritionDiariesQuery";
import { setNutritionOverviewQueryData } from "../queries/useNutritionOverviewQuery";
import recipeDiaryAddedToast from "../toasts/recipeDiaryAddedToast";
import { invalidateDailyNutritionOverviewsQuery } from "../../overview/queries/useDailyNutritionOverviewsQuery";

const recipeDiariesApi = new RecipeDiariesApi();

export interface AddRecipeDiaryMutationVariables extends AddRecipeDiaryRequest {
  recipe: RecipeDto;
}

export default function useAddRecipeDiaryMutation() {
  const addRecipeDiaryMutation = useMutation({
    mutationFn: (variables: AddRecipeDiaryMutationVariables) => {
      const { ...addRecipeDiaryRequest } = variables;
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

      invalidateDailyNutritionOverviewsQuery()

      setNutritionOverviewQueryData({
        adjustment: multiplyNutritionalContent(
          recipe.nutritionalContents,
          (1 / recipe.portions) * variables.quantity,
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
          name: recipe.name,
          nutritionalContents: multiplyNutritionalContent(
            recipe.nutritionalContents,
            (1 / recipe.portions) * variables.quantity,
          ),
          nutritionMultiplier: variables.quantity,
          diaryType: DiaryType.FoodDiary,
          quantity: variables.quantity,
          mealType: variables.mealType,
          date: variables.entryDate,
          isLoading: true,
          isDeleting: false,
        },
        invalidate: true,
      });
    },
    onError: toastDefaultServerError,
  });

  const isPending = useDelayedLoading(addRecipeDiaryMutation.isPending);

  return { addRecipeDiaryMutation, isPending };
}
