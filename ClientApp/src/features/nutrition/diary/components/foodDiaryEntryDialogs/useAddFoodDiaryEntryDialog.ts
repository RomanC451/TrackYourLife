import { useToggle } from "usehooks-ts";

import { DateOnly } from "@/lib/date";
import { FoodDto, MealTypes } from "@/services/openapi";

import { AddFoodDiaryFormSchema } from "../../data/schemas";
import useAddFoodDiaryMutation from "../../mutations//useAddFoodDiaryMutation";

const useAddFoodDiaryEntryDialog = (
  food: FoodDto,
  date: DateOnly,
  onSuccess?: () => void,
) => {
  const [dialogState, toggleDialogState] = useToggle(false);

  const { addFoodDiaryMutation, isPending } = useAddFoodDiaryMutation();

  function onSubmit(formData: AddFoodDiaryFormSchema) {
    addFoodDiaryMutation.mutate(
      {
        foodId: food.id,
        mealType: formData.mealType as MealTypes,
        servingSizeId: food.servingSizes[formData.servingSizeIndex].id,
        servingSize: food.servingSizes[formData.servingSizeIndex],
        quantity: formData.nrOfServings,
        entryDate: date,
        food: food,
      },
      {
        onSuccess: () => {
          onSuccess?.();
          toggleDialogState();
        },
      },
    );
  }

  return {
    dialogState,
    toggleDialogState,
    onSubmit,
    isPending,
  };
};

export default useAddFoodDiaryEntryDialog;
