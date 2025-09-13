import { useCustomMutation } from "@/hooks/useCustomMutation";
import { DateOnly } from "@/lib/date";
import { queryClient } from "@/queryClient";
import { RecipeDiariesApi, UpdateRecipeDiaryRequest } from "@/services/openapi";

import {
  nutritionDiariesQueryKeys,
  setNutritionDiariesQueryData,
} from "../queries/useDiaryQuery";

const recipeDiariesApi = new RecipeDiariesApi();

export type UpdateRecipeDiaryMutationVariables = UpdateRecipeDiaryRequest & {
  id: string;
};

export default function useUpdateRecipeDiaryMutation() {
  const updateRecipeDiaryMutation = useCustomMutation({
    mutationFn: (variables: UpdateRecipeDiaryMutationVariables) => {
      const { id, ...updateRecipeDiaryRequest } = variables;
      return recipeDiariesApi.updateRecipeDiary(id, updateRecipeDiaryRequest);
    },
    meta: {
      invalidateQueries: [],
      onSuccessToast: {
        type: "success",
        message: "Recipe diary updated successfully",
      },
    },
    onSuccess: (_, variables) => {
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

      setNutritionDiariesQueryData({
        date: variables.entryDate as DateOnly,
        mealType: variables.mealType,
        updatedDiary: {
          id: variables.id,
          quantity: variables.quantity,
          mealType: variables.mealType,
        },
      });
    },
  });

  return updateRecipeDiaryMutation;
}
