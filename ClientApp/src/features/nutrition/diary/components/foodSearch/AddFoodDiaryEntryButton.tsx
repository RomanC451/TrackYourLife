import { DateOnly } from "@/lib/date";
import { FoodDto, MealTypes } from "@/services/openapi";

import MealTypeDropDownMenu from "../../../common/components/formFields/MealTypeDropDownMenu";
import useAddFoodDiaryMutation from "../../mutations/useAddFoodDiaryMutation";

type AddFoodDiaryEntryButtonProps = {
  food: FoodDto;
  date: DateOnly;
  className?: string;
};

function AddFoodDiaryEntryButton({
  food,
  date,
  className,
}: AddFoodDiaryEntryButtonProps) {
  const addFoodDiaryMutation = useAddFoodDiaryMutation(food);

  async function addFoodToMeal(meal: MealTypes) {
    addFoodDiaryMutation.mutate({
      foodId: food.id,
      mealType: meal,
      servingSizeId: food.servingSizes[0].id,
      quantity: 1,
      entryDate: date,
    });
  }

  return (
    <MealTypeDropDownMenu
      selectCallback={addFoodToMeal}
      date={date}
      pendingState={addFoodDiaryMutation.pendingState}
      className={className}
    />
  );
}

export default AddFoodDiaryEntryButton;
