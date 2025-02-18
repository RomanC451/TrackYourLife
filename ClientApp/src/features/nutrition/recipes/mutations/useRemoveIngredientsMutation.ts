import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { queryClient } from "@/queryClient";
import { IngredientDto, RecipeDto, RecipesApi } from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { QUERY_KEYS } from "../../common/data/queryKeys";
import { setRecipesQueryData } from "../../common/queries/useRecipesQuery";
import { removeIngredientFromRecipe } from "../../common/utils/recipes";
import {
  getRecipeQueryData,
  invalidateRecipeQuery,
  setRecipeQueryData,
} from "../queries/useRecipeQuery";
import ingredientRemovedToast from "../toasts/ingredientRemovedToast";

const recipesApi = new RecipesApi();

type RemoveIngredientsMutationVariables = {
  recipe: RecipeDto;
  ingredients: IngredientDto[];
};

export default function useRemoveIngredientsMutation() {
  const removeIngredientsMutation = useMutation({
    mutationFn: ({
      recipe,
      ingredients,
    }: RemoveIngredientsMutationVariables) => {
      const promise = recipesApi
        .removeIngredients(recipe.id, {
          ingredientsIds: ingredients.map((i) => i.id),
        })
        .then((res) => res.data);

      ingredientRemovedToast(promise);

      return promise;
    },

    onMutate: ({ recipe, ingredients }) => {
      queryClient.cancelQueries({ queryKey: [QUERY_KEYS.recipes, recipe.id] });

      const previousRecipe = getRecipeQueryData(recipe.id);

      if (!previousRecipe) {
        return { previousRecipe };
      }

      setRecipeQueryData({
        recipeId: recipe.id,
        removedIngredientsIds: ingredients.map((i) => i.id),
      });

      return { previousRecipe };
    },

    onSuccess: (_, { recipe, ingredients }) => {
      setRecipesQueryData({
        updatedRecipe: removeIngredientFromRecipe(
          recipe,
          ingredients.map((i) => i.id),
        ),
        invalidate: true,
      });

      invalidateRecipeQuery(recipe.id);
    },
    onError: (error, _, context) => {
      toastDefaultServerError(error);
      if (!context) {
        return;
      }

      const { previousRecipe } = context;
      if (!previousRecipe) {
        return;
      }

      setRecipeQueryData({
        recipeId: previousRecipe.id,
        data: previousRecipe,
        invalidate: true,
      });
    },
    onSettled: (_, _2, { recipe }) => {
      invalidateRecipeQuery(recipe.id);
    },
  });

  const isPending = useDelayedLoading(removeIngredientsMutation.isPending);

  return { removeIngredientsMutation, isPending };
}
