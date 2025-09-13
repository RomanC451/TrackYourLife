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
  const newRecipe = structuredClone({ ...recipe, isLoading: true });

  const existingIngredient = newRecipe.ingredients.find(
    (ingredient) =>
      ingredient.food.name + ingredient.food.brandName ===
      addedIngredient.food.name + addedIngredient.food.brandName,
  );

  if (existingIngredient) {
    if (existingIngredient.servingSize.id !== addedIngredient.servingSize.id) {
      return;
    }
    addedIngredient.quantity += existingIngredient.quantity;

    addedIngredient.id = existingIngredient.id;

    return updateIngredientInRecipe(newRecipe, addedIngredient);
  }

  newRecipe.ingredients.push(addedIngredient);
  newRecipe.nutritionalContents = addNutritionalContent(
    newRecipe.nutritionalContents,
    multiplyNutritionalContent(
      addedIngredient.food.nutritionalContents,
      addedIngredient.quantity *
        addedIngredient.servingSize.nutritionMultiplier,
    ),
  );

  return newRecipe;
}

export function updateIngredientInRecipe(
  recipe: RecipeDto,
  updatedIngredient: IngredientDto,
) {
  const newRecipe = structuredClone({ ...recipe, isLoading: true });

  for (let index = 0; index < newRecipe.ingredients.length; index++) {
    const ingredient = newRecipe.ingredients[index];
    if (
      ingredient.food.name + ingredient.food.brandName ===
      updatedIngredient.food.name + updatedIngredient.food.brandName
    ) {
      const tmp = subtractNutritionalContent(
        newRecipe.nutritionalContents,
        multiplyNutritionalContent(
          ingredient.food.nutritionalContents,
          ingredient.quantity * ingredient.servingSize.nutritionMultiplier,
        ),
      );

      newRecipe.nutritionalContents = addNutritionalContent(
        tmp,
        multiplyNutritionalContent(
          updatedIngredient.food.nutritionalContents,
          updatedIngredient.quantity *
            updatedIngredient.servingSize.nutritionMultiplier,
        ),
      );
      newRecipe.ingredients[index] = updatedIngredient;
    }
  }

  return newRecipe;
}

export function removeIngredientFromRecipe(
  recipe: RecipeDto,
  removedIngredientsIds: string[],
) {
  const newRecipe = structuredClone({ ...recipe, isLoading: true });

  const removedIngredients = newRecipe.ingredients.filter((ingredient) =>
    removedIngredientsIds.includes(ingredient.id),
  );

  if (removedIngredients) {
    for (const removedIngredient of removedIngredients) {
      newRecipe.nutritionalContents = subtractNutritionalContent(
        newRecipe.nutritionalContents,
        multiplyNutritionalContent(
          removedIngredient.food.nutritionalContents,
          removedIngredient.quantity *
            removedIngredient.servingSize.nutritionMultiplier,
        ),
      );

      // ?? FIXME:  is it correct?
      // newRecipe.nutritionalContents = subtractNutritionalContent(
      //   newRecipe.nutritionalContents,
      //   removedIngredient.food.nutritionalContents,
      // );
    }
  }

  newRecipe.ingredients = newRecipe.ingredients.filter(
    (ingredient) => !removedIngredientsIds.includes(ingredient.id),
  );

  return newRecipe;
}
