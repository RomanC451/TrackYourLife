import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";
import { toastDefaultServerError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { IngredientDto, RecipeDto, RecipesApi } from "~/services/openapi";
import {
  getRecipeQueryData,
  invalidateRecipeQuery,
  setRecipeQueryData,
} from "../../queries/recipes/useRecipeQuery";
import { setRecipesQueryData } from "../../queries/recipes/useRecipesQuery";
import { removeIngredientFromRecipe } from "../../utils/recipes";

const recipesApi = new RecipesApi();

type RemoveIngredientMutationVariables = {
  recipe: RecipeDto;
  ingredient: IngredientDto;
};

export default function useRemoveIngredientMutation() {
  const removeIngredientMutation = useMutation({
    mutationFn: ({ recipe, ingredient }: RemoveIngredientMutationVariables) => {
      const promise = recipesApi
        .removeIngredient(recipe.id, ingredient.id)
        .then((res) => res.data);

      toast.promise(promise, {
        loading: `Removing ${ingredient.food.name}...`,
        success: () => `${ingredient.food.name} has been removed`,
        error: (err) =>
          err?.response?.data?.detail ?? "Failed to remove ingredient",
      });

      return promise;
    },

    // recipesApi
    //   .removeIngredient(recipe.id, ingredient.id)
    //   .then((res) => res.data),

    onMutate: ({ recipe, ingredient }) => {
      const previousRecipe = getRecipeQueryData(recipe.id);

      if (!previousRecipe) {
        return { previousRecipe };
      }

      setRecipeQueryData({
        recipeId: recipe.id,
        removedIngredientId: ingredient.id,
      });

      return { previousRecipe };
    },

    onSuccess: (_, { recipe, ingredient }) => {
      // ingredientRemovedToast(ingredient.food.name);
      setRecipesQueryData({
        updatedRecipe: removeIngredientFromRecipe(recipe, ingredient.id),
        invalidate: true,
      });
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

      setRecipeQueryData({ recipeId: previousRecipe.id, data: previousRecipe });
    },
    onSettled: (_, _2, { recipe }) => {
      invalidateRecipeQuery(recipe.id);
    },
  });

  const isPending = useDelayedLoading(removeIngredientMutation.isPending);

  // console.log(removeIngredientMutation.isPending);

  // useStoreLoadingStateToContext(
  //   "removeIngredientMutation",
  //   removeIngredientMutation.isPending,
  // );

  return { removeIngredientMutation, isPending };
}
