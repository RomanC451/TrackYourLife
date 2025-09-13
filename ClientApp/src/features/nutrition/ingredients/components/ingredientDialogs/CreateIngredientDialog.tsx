import { FoodDto, RecipeDto } from "@/services/openapi";

import IngredientDialog from "./IngredientDialog";

function CreateIngredientDialog({
  recipe,
  food,
  onSuccess,
  onClose,
}: {
  recipe: RecipeDto;
  food: FoodDto;
  onSuccess: () => void;
  onClose: () => void;
}) {
  return (
    <IngredientDialog
      dialogType="create"
      food={food}
      recipe={recipe}
      servingSizes={Object.values(food.servingSizes)}
      onClose={onClose}
      onSuccess={onSuccess}
    />
  );
}

export default CreateIngredientDialog;
