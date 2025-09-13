import { useSuspenseQuery } from "@tanstack/react-query";
import { useLocalStorage } from "usehooks-ts";

import { foodQueryOptions } from "@/features/nutrition/common/queries/useFoodQuery";
import { getDateOnly } from "@/lib/date";
import { MealTypes } from "@/services/openapi";

import useAddFoodDiaryMutation from "../../mutations/useAddFoodDiaryMutation";
import { FoodDiaryDialog } from "./FoodDiaryDialog";

export function CreateFoodDiaryDialog({
  foodId,
  onSuccess,
  onClose,
}: {
  foodId: string;
  onSuccess: () => void;
  onClose: () => void;
}) {
  const { data: food } = useSuspenseQuery(foodQueryOptions.byId(foodId));

  const addFoodDiaryMutation = useAddFoodDiaryMutation(food);

  const [lastMealType] = useLocalStorage("lastMealType", "");

  return (
    <FoodDiaryDialog
      dialogType="create"
      mutation={addFoodDiaryMutation}
      defaultValues={{
        foodId: foodId,
        mealType: lastMealType as MealTypes,
        servingSizeId: food.servingSizes[0].id,
        quantity: 1,
        entryDate: getDateOnly(new Date()),
      }}
      food={food}
      onClose={onClose}
      onSuccess={onSuccess}
    />
  );
}
