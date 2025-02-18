import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import {
  IngredientDto,
  RecipeDto,
  RecipesApi,
  ServingSizeDto,
  UpdateIngredientRequest,
} from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { setRecipesQueryData } from "../../common/queries/useRecipesQuery";
import { updateIngredientInRecipe } from "../../common/utils/recipes";
import { setRecipeQueryData } from "../queries/useRecipeQuery";
import ingredientUpdatedToast from "../toasts/ingredientUpdatedToast";

const recipesApi = new RecipesApi();

type UpdateIngredientMutationVariables = {
  recipe: RecipeDto;
  ingredient: IngredientDto;
  servingSize: ServingSizeDto;
} & UpdateIngredientRequest;

export default function useUpdateIngredientMutation() {
  const updateIngredientMutation = useMutation({
    mutationFn: ({
      recipe,
      ingredient,
      ...request
    }: UpdateIngredientMutationVariables) =>
      recipesApi
        .updateIngredient(recipe.id, ingredient.id, request)
        .then((resp) => resp.data),
    onSuccess: (_, { recipe, ingredient, quantity, servingSize }) => {
      ingredientUpdatedToast(ingredient.food.name);
      setRecipeQueryData({
        recipeId: recipe.id,
        updatedIngredient: {
          ...ingredient,
          quantity,
          servingSize,
        },
        invalidate: true,
      });
      setRecipesQueryData({
        updatedRecipe: updateIngredientInRecipe(recipe, {
          ...ingredient,
          quantity,
          servingSize,
        }),
        invalidate: true,
      });
    },
    onError: toastDefaultServerError,
  });

  const isPending = useDelayedLoading(updateIngredientMutation.isPending);

  return { updateIngredientMutation, isPending };
}
