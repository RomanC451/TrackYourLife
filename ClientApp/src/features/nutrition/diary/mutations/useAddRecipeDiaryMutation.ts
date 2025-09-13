import { useCustomMutation } from "@/hooks/useCustomMutation";
import { DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import {
  AddRecipeDiaryRequest,
  DiaryType,
  NutritionalContent,
  RecipeDiariesApi,
  RecipeDto,
} from "@/services/openapi";

import {
  addNutritionalContent,
  multiplyNutritionalContent,
} from "../../common/utils/nutritionalContent";
import { dailyNutritionOverviewsQueryKeys } from "../../overview/queries/useDailyNutritionOverviewsQuery";
import {
  nutritionDiariesQueryKeys,
  setNutritionDiariesQueryData,
} from "../queries/useDiaryQuery";

const recipeDiariesApi = new RecipeDiariesApi();

export type AddRecipeDiaryMutationVariables = AddRecipeDiaryRequest;

export default function useAddRecipeDiaryMutation(recipe: RecipeDto) {
  const addRecipeDiaryMutation = useCustomMutation({
    mutationFn: (variables: AddRecipeDiaryMutationVariables) => {
      const { ...addRecipeDiaryRequest } = variables;
      return recipeDiariesApi
        .addRecipeDiary(addRecipeDiaryRequest)
        .then((resp) => resp.data);
    },

    meta: {
      invalidateQueries: [dailyNutritionOverviewsQueryKeys.all],
      onSuccessToast: {
        type: "success",
        message: "Recipe diary added successfully",
      },
    },
    onSuccess: (resp, variables) => {
      queryClient.setQueryData(
        nutritionDiariesQueryKeys.overview(
          variables.entryDate as DateOnly,
          variables.entryDate as DateOnly,
        ),
        (oldData: NutritionalContent) =>
          addNutritionalContent(
            oldData,
            multiplyNutritionalContent(
              recipe.nutritionalContents,
              (1 / recipe.portions) * variables.quantity,
            ),
          ),
      );

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
          diaryType: DiaryType.RecipeDiary,
          quantity: variables.quantity,
          mealType: variables.mealType,
          date: variables.entryDate,
          isLoading: true,
          isDeleting: false,
        },
      });
    },
    onSettled: () => {
      queryClient.invalidateQueries({
        queryKey: nutritionDiariesQueryKeys.all,
      });
    },
  });

  return addRecipeDiaryMutation;
}
