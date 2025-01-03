import { DateOnly } from "@/lib/date";
import { MealTypes } from "@/services/openapi";

import { AddRecipeDiaryFormSchema } from "../../data/schemas";
import useUpdateRecipeDiaryMutation from "../../mutations/useUpdateRecipeDiaryMutation";
import useRecipeDiaryQuery, {
  invalidateRecipeDiaryQuery,
} from "../../queries/useRecipeDiaryQuery";

function useEditRecipeDiaryEntryDialog(
  diaryId: string,
  toggleDialogState: () => void,
) {
  const recipeDiaryQuery = useRecipeDiaryQuery(diaryId);

  const { updateRecipeDiaryMutation, isPending } =
    useUpdateRecipeDiaryMutation();

  function onSubmit(formData: AddRecipeDiaryFormSchema) {
    if (recipeDiaryQuery.data == undefined) return;

    const diary = recipeDiaryQuery.data;

    updateRecipeDiaryMutation.mutate(
      {
        id: diary.id,
        mealType: formData.mealType as MealTypes,
        quantity: formData.nrOfServings,
        date: diary.date as DateOnly,
      },
      {
        onSuccess: () => {
          toggleDialogState();
          invalidateRecipeDiaryQuery(diary.id);
        },
      },
    );
  }

  return {
    recipeDiaryQuery,
    onSubmit,
    isPending,
  };
}

export default useEditRecipeDiaryEntryDialog;
