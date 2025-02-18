import { useToggle } from "usehooks-ts";

import { FoodDto, RecipeDto } from "@/services/openapi";

import { AddIngredientFormSchema } from "../../data/schemas";
import useAddIngredientMutation from "../../mutations/useAddIngredientMutation";

export default function useAddIngredientDialog(
  food: FoodDto,
  recipe: RecipeDto,
  onSuccess?: () => void,
) {
  const [dialogState, toggleDialogState] = useToggle(false);

  const { addIngredientMutation, isPending } = useAddIngredientMutation();

  function onSubmit(formData: AddIngredientFormSchema) {
    addIngredientMutation.mutate(
      {
        recipe,
        food: food,
        quantity: formData.nrOfServings,
        foodId: food.id,
        servingSizeId: food.servingSizes[formData.servingSizeIndex].id,
        servingSize: food.servingSizes[formData.servingSizeIndex],
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
}
