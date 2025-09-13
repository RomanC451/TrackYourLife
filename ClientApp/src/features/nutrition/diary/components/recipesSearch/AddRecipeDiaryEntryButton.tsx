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
  const addRecipeDiaryMutation = useAddRecipeDiaryMutation(recipe);

  async function addRecipeToMeal(meal: MealTypes) {
    addRecipeDiaryMutation.mutate({
      recipeId: recipe.id,
      mealType: meal,
      quantity: 1,
      entryDate: date,
      servingSizeId: recipe.servingSizes[0].id,
    });
  }

  return (
    <MealTypeDropDownMenu
      selectCallback={addRecipeToMeal}
      date={date}
      className={className}
      pendingState={addRecipeDiaryMutation.pendingState}
    />
  );
}

export default AddRecipeDiaryEntryButton;
