import { DateOnly } from "@/lib/date";
import { MealTypes, RecipeDto } from "@/services/openapi";

import MealTypeDropDownMenu from "../../../common/components/formFields/MealTypeDropDownMenu";
import useAddRecipeDiaryMutation from "../../mutations/useAddRecipeDiaryMutation";

type AddRecipeDiaryEntryButtonProps = {
  recipe: RecipeDto;
  date: DateOnly;
  className?: string;
};

function AddRecipeDiaryEntryButton({
  recipe,
  date,
  className,
}: AddRecipeDiaryEntryButtonProps) {
  const { addRecipeDiaryMutation, isPending } = useAddRecipeDiaryMutation();

  async function addRecipeToMeal(meal: MealTypes) {
    addRecipeDiaryMutation.mutate({
      recipeId: recipe.id,
      mealType: meal,
      quantity: 1,
      entryDate: date,
      recipe: recipe,
    });
  }

  return (
    <MealTypeDropDownMenu
      selectCallback={addRecipeToMeal}
      date={date}
      className={className}
      isPending={isPending}
    />
  );
}

export default AddRecipeDiaryEntryButton;
