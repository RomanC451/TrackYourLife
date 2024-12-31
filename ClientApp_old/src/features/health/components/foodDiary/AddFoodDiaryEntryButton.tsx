import { FoodDto, MealTypes } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import useAddFoodDiaryMutation from "../../mutations/foodDiaries/useAddFoodDiaryMutation";
import MealTypeDropDownMenu from "./MealTypeDropDownMenu";

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
  const { addFoodDiaryMutation, isPending } = useAddFoodDiaryMutation();

  async function addFoodToMeal(meal: MealTypes) {
    addFoodDiaryMutation.mutate({
      foodId: food.id,
      mealType: meal,
      servingSizeId: food.servingSizes[0].id,
      servingSize: food.servingSizes[0],
      quantity: 1,
      entryDate: date,
      food: food,
    });
  }

  return (
    <MealTypeDropDownMenu
      selectCallback={addFoodToMeal}
      date={date}
      isPending={isPending}
      className={className}
    />
  );
}

export default AddFoodDiaryEntryButton;
