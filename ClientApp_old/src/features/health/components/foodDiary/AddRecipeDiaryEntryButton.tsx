import { MealTypes, RecipeDto } from "~/services/openapi";
import { DateOnly } from "~/utils/date";
import useAddRecipeDiaryMutation from "../../mutations/foodDiaries/useAddRecipeDiaryMutation";
import MealTypeDropDownMenu from "./MealTypeDropDownMenu";

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
    <>
      <MealTypeDropDownMenu
        selectCallback={addRecipeToMeal}
        date={date}
        className={className}
        isPending={isPending}
      />
    </>
  );
}

export default AddRecipeDiaryEntryButton;
