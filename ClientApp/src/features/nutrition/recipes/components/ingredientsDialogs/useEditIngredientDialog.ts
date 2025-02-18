import { useToggle } from "usehooks-ts";

import { IngredientDto, RecipeDto } from "@/services/openapi";

import { AddIngredientFormSchema } from "../../data/schemas";
import useUpdateIngredientMutation from "../../mutations/useUpdateIngredientMutation";

export default function useEditIngredientDialog(
  recipe: RecipeDto,
  ingredient: IngredientDto,
) {
  const [dialogState, toggleDialogState] = useToggle();

  const { updateIngredientMutation, isPending } = useUpdateIngredientMutation();

  function onSubmit(formData: AddIngredientFormSchema) {
    updateIngredientMutation.mutate(
      {
        recipe,
        ingredient,
        quantity: formData.nrOfServings,
        servingSizeId:
          ingredient.food.servingSizes[formData.servingSizeIndex].id,
        servingSize: ingredient.food.servingSizes[formData.servingSizeIndex],
      },
      {
        onSuccess: () => {
          toggleDialogState();
        },
      },
    );
  }

  return { onSubmit, isPending, dialogState, toggleDialogState };
}
