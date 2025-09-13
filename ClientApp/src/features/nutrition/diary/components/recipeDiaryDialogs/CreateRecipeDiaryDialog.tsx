import { useSuspenseQuery } from "@tanstack/react-query";
import { useReadLocalStorage } from "usehooks-ts";

import { recipesQueryOptions } from "@/features/nutrition/recipes/queries/useRecipeQuery";
import { getDateOnly } from "@/lib/date";
import { MealTypes } from "@/services/openapi";

import useAddRecipeDiaryMutation from "../../mutations/useAddRecipeDiaryMutation";
import { RecipeDiaryDialog } from "./RecipeDiaryDialog";

export function CreateRecipeDiaryDialog({
  recipeId,
  onSuccess,
  onClose,
}: {
  recipeId: string;
  onSuccess: () => void;
  onClose: () => void;
}) {
  const { data: recipe } = useSuspenseQuery(recipesQueryOptions.byId(recipeId));

  const addRecipeDiaryMutation = useAddRecipeDiaryMutation(recipe);

  const mealType = useReadLocalStorage<MealTypes>("lastMealType");

  return (
    <RecipeDiaryDialog
      dialogType="create"
      mutation={addRecipeDiaryMutation}
      defaultValues={{
        recipeId: recipeId,
        mealType: mealType as MealTypes,
        servingSizeId: recipe.servingSizes[0].id,
        quantity: 1,
        entryDate: getDateOnly(new Date()),
      }}
      onClose={onClose}
      onSuccess={onSuccess}
      recipe={recipe}
    />
  );
}
