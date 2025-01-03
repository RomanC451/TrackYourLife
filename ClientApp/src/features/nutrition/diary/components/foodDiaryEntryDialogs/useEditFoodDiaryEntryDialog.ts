import { DateOnly } from "@/lib/date";
import { MealTypes } from "@/services/openapi";

import { AddFoodDiaryFormSchema } from "../../data/schemas";
import useUpdateFoodDiaryMutation from "../../mutations/useUpdateFoodDiaryMutation";
import useFoodDiaryQuery, {
  invalidateFoodDiaryQuery,
} from "../../queries/useFoodDiaryQuery";

const useEditFoodDiaryEntryDialog = (
  diaryId: string,
  toggleDialogState: () => void,
) => {
  const foodDiaryQuery = useFoodDiaryQuery(diaryId);

  const { updateFoodDiaryMutation, isPending } = useUpdateFoodDiaryMutation();

  function onSubmit(formData: AddFoodDiaryFormSchema) {
    if (foodDiaryQuery.data == undefined) return;

    const diary = foodDiaryQuery.data;

    const servingSize = diary.food.servingSizes[formData.servingSizeIndex];

    updateFoodDiaryMutation.mutate(
      {
        id: diary.id,
        mealType: formData.mealType as MealTypes,
        servingSizeId: servingSize.id,
        quantity: formData.nrOfServings,
        date: diary.date as DateOnly,
        servingSize: servingSize,
      },
      {
        onSuccess: () => {
          toggleDialogState();
          invalidateFoodDiaryQuery(diary.id);
        },
      },
    );
  }

  return {
    foodDiaryQuery,
    onSubmit,
    isPending,
  };
};

export default useEditFoodDiaryEntryDialog;
