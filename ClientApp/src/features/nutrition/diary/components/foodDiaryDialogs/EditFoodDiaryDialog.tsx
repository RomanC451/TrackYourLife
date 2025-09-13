import { useSuspenseQuery } from "@tanstack/react-query";

import { DateOnly } from "@/lib/date";

import useUpdateFoodDiaryMutation from "../../mutations/useUpdateFoodDiaryMutation";
import { foodDiariesQueryOptions } from "../../queries/useDiaryQuery";
import { FoodDiaryDialog } from "./FoodDiaryDialog";

export function EditFoodDiaryDialog({
  foodDiaryId,
  onSuccess,
  onClose,
}: {
  foodDiaryId: string;
  onSuccess: () => void;
  onClose: () => void;
}) {
  const { data: foodDiary } = useSuspenseQuery(
    foodDiariesQueryOptions.byId(foodDiaryId),
  );
  const updateFoodDiaryMutation = useUpdateFoodDiaryMutation(foodDiary.food);

  return (
    <FoodDiaryDialog
      dialogType="edit"
      mutation={updateFoodDiaryMutation}
      defaultValues={{
        id: foodDiaryId,
        foodId: foodDiary.food.id,
        servingSizeId: foodDiary.servingSize.id,
        quantity: foodDiary.quantity,
        entryDate: foodDiary.date as DateOnly,
        mealType: foodDiary.mealType,
      }}
      food={foodDiary.food}
      onClose={onClose}
      onSuccess={onSuccess}
    />
  );
}
