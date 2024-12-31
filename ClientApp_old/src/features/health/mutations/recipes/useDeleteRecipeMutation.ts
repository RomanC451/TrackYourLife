import { useMutation } from "@tanstack/react-query";
import { toastDefaultServerError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { RecipeDto, RecipesApi } from "~/services/openapi";
import { setRecipesQueryData } from "../../queries/recipes/useRecipesQuery";
import recipeDeletedToast from "../../toasts/recipes/recipeDeletedToast";

const recipesApi = new RecipesApi();

const useDeleteRecipeMutation = () => {
  const deleteRecipeMutation = useMutation({
    mutationFn: (recipe: RecipeDto) => {
      return recipesApi.deleteRecipe(recipe.id);
    },
    onSuccess: (_, recipe) => {
      recipeDeletedToast({
        name: recipe.name,
        action: () => {
          // TODO: Implement undo functionality
        },
      });

      setRecipesQueryData({
        filter: (entry) => entry.id !== recipe.id,
        invalidate: true,
      });
    },
    onError: toastDefaultServerError,
  });

  const isPending = useDelayedLoading(deleteRecipeMutation.isPending);

  return { deleteRecipeMutation, isPending };
};

export default useDeleteRecipeMutation;
