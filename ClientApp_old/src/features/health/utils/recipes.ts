import { IngredientDto, RecipeDto } from "~/services/openapi";
import {
  addNutritionalContent,
  multiplyNutritionalContent,
  subtractNutritionalContent,
} from "./nutritionalContent";

export function addIngredientInRecipe(
  recipe: RecipeDto,
  addedIngredient: IngredientDto,
) {
  recipe.ingredients.push(addedIngredient);
  recipe.nutritionalContents = addNutritionalContent(
    recipe.nutritionalContents,
    addedIngredient.food.nutritionalContents,
  );

  return recipe;
}

export function updateIngredientInRecipe(
  recipe: RecipeDto,
  updatedIngredient: IngredientDto,
) {
  recipe.ingredients = recipe.ingredients.map((ingredient) => {
    if (ingredient.id === updatedIngredient.id) {
      const tmp = subtractNutritionalContent(
        recipe.nutritionalContents,
        multiplyNutritionalContent(
          updatedIngredient.food.nutritionalContents,
          ingredient.quantity * ingredient.servingSize.nutritionMultiplier,
        ),
      );

      recipe.nutritionalContents = addNutritionalContent(
        tmp,
        multiplyNutritionalContent(
          updatedIngredient.food.nutritionalContents,
          updatedIngredient.quantity *
            updatedIngredient.servingSize.nutritionMultiplier,
        ),
      );
      return updatedIngredient;
    }
    return ingredient;
  });

  return recipe;
}

export function removeIngredientFromRecipe(
  recipe: RecipeDto,
  removedIngredientId: string,
) {
  const removedIngredient = recipe.ingredients.find(
    (ingredient) => ingredient.id === removedIngredientId,
  );

  if (removedIngredient) {
    recipe.nutritionalContents = subtractNutritionalContent(
      recipe.nutritionalContents,
      removedIngredient.food.nutritionalContents,
    );
  }

  recipe.ingredients = recipe.ingredients.filter(
    (ingredient) => ingredient.id !== removedIngredientId,
  );

  return recipe;
}
