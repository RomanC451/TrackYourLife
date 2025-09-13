import { IngredientDto, RecipeDto } from "@/services/openapi";

import IngredientDialog from "./IngredientDialog";

function EditIngredientDialog({
  recipe,
  ingredient,
  onSuccess,
  onClose,
}: {
  recipe: RecipeDto;
  ingredient: IngredientDto;
  onSuccess: () => void;
  onClose: () => void;
}) {
  return (
    <IngredientDialog
      dialogType="edit"
      ingredient={ingredient}
      food={ingredient.food}
      recipe={recipe}
      servingSizes={Object.values(ingredient.food.servingSizes)}
      onSuccess={onSuccess}
      onClose={onClose}
    />
  );
}

export default EditIngredientDialog;
