import { useMutation } from "@tanstack/react-query";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { RecipeDiariesApi, UpdateRecipeDiaryRequest } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import {
  invalidateNutritionDiariesQuery,
  setNutritionDiariesQueryData,
} from "../../queries/foodDiaries/useNutritionDiariesQuery";

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
