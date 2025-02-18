import { IngredientDto, RecipeDto } from "@/services/openapi";

import {
  addNutritionalContent,
  multiplyNutritionalContent,
  subtractNutritionalContent,
} from "./nutritionalContent";

export function addIngredientInRecipe(
  recipe: RecipeDto,
  addedIngredient: IngredientDto,
) {
  const existingIngredient = recipe.ingredients.find(
    (ingredient) =>
      ingredient.food.name + ingredient.food.brandName ===
      addedIngredient.food.name + addedIngredient.food.brandName,
  );

  if (existingIngredient) {
    if (existingIngredient.servingSize.id !== addedIngredient.servingSize.id) {
      return;
    }
    addedIngredient.quantity += existingIngredient.quantity;

    return updateIngredientInRecipe(recipe, addedIngredient);
  }

  recipe.ingredients.push(addedIngredient);
  recipe.nutritionalContents = addNutritionalContent(
    recipe.nutritionalContents,
    multiplyNutritionalContent(
      addedIngredient.food.nutritionalContents,
      addedIngredient.quantity *
        addedIngredient.servingSize.nutritionMultiplier,
    ),
  );

  return recipe;
}

export function updateIngredientInRecipe(
  recipe: RecipeDto,
  updatedIngredient: IngredientDto,
) {
  for (let index = 0; index < recipe.ingredients.length; index++) {
    const ingredient = recipe.ingredients[index];
    if (
      ingredient.food.name + ingredient.food.brandName ===
      updatedIngredient.food.name + updatedIngredient.food.brandName
    ) {
      const tmp = subtractNutritionalContent(
        recipe.nutritionalContents,
        multiplyNutritionalContent(
          ingredient.food.nutritionalContents,
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
      recipe.ingredients[index] = updatedIngredient;
    }
  }

  return recipe;
}

export function removeIngredientFromRecipe(
  recipe: RecipeDto,
  removedIngredientsIds: string[],
) {
  const removedIngredients = recipe.ingredients.filter((ingredient) =>
    removedIngredientsIds.includes(ingredient.id),
  );

  if (removedIngredients) {
    for (const removedIngredient of removedIngredients) {
      recipe.nutritionalContents = subtractNutritionalContent(
        recipe.nutritionalContents,
        multiplyNutritionalContent(
          removedIngredient.food.nutritionalContents,
          removedIngredient.quantity *
            removedIngredient.servingSize.nutritionMultiplier,
        ),
      );

      // ?? FIXME:  is it correct?
      // recipe.nutritionalContents = subtractNutritionalContent(
      //   recipe.nutritionalContents,
      //   removedIngredient.food.nutritionalContents,
      // );
    }
  }

  recipe.ingredients = recipe.ingredients.filter(
    (ingredient) => !removedIngredientsIds.includes(ingredient.id),
  );

  return recipe;
}
