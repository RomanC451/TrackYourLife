import { MealTypes } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import { AddFoodDiaryFormSchema } from "../../data/schemas";
import useUpdateFoodDiaryMutation from "../../mutations/foodDiaries/useUpdateFoodDiaryMutation";
import useFoodDiaryQuery, {
  invalidateFoodDiaryQuery,
} from "../../queries/foodDiaries/useFoodDiaryQuery";

const useUpdateFoodDiaryEntryDialog = (
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

export default useUpdateFoodDiaryEntryDialog;
