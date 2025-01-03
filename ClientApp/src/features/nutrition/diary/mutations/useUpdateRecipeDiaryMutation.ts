import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DateOnly } from "@/lib/date";
import { RecipeDiariesApi, UpdateRecipeDiaryRequest } from "@/services/openapi";

import {
  invalidateNutritionDiariesQuery,
  setNutritionDiariesQueryData,
} from "../queries/useNutritionDiariesQuery";

const recipeDiariesApi = new RecipeDiariesApi();

interface UpdateRecipeDiaryMutationVariables extends UpdateRecipeDiaryRequest {
  date: DateOnly;
}

export default function useUpdateRecipeDiaryMutation() {
  const updateRecipeDiaryMutation = useMutation({
    mutationFn: (variables: UpdateRecipeDiaryMutationVariables) =>
      recipeDiariesApi.updateRecipeDiary(variables),
    onSuccess: (_, variables) => {
      invalidateNutritionDiariesQuery();

      setNutritionDiariesQueryData({
        date: variables.date,
        mealType: variables.mealType,
        updatedDiary: {
          id: variables.id,
          quantity: variables.quantity,
          mealType: variables.mealType,
        },
        invalidate: true,
      });
    },
  });

  const isPending = useDelayedLoading(updateRecipeDiaryMutation.isPending);

  return { updateRecipeDiaryMutation, isPending };
}
