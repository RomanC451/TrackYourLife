import { IngredientDto } from "@/services/openapi";

export function getCalories(ingredient: IngredientDto) {
  return (
    ingredient.servingSize.nutritionMultiplier *
    ingredient.quantity *
    ingredient.food.nutritionalContents.energy.value
  );
}
