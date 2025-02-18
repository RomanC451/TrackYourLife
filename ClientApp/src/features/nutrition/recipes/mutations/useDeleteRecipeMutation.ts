import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { RecipeDto, RecipesApi } from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { setRecipesQueryData } from "../../common/queries/useRecipesQuery";
import recipeDeletedToast from "../toasts/recipeDeletedToast";
import useUndoDeleteRecipeMutation from "./useUndoDeleteRecipeMutation";

const recipesApi = new RecipesApi();

const useDeleteRecipeMutation = () => {
  const { undoDeleteRecipeMutation } = useUndoDeleteRecipeMutation();

  const deleteRecipeMutation = useMutation({
    mutationFn: (recipe: RecipeDto) => {
      return recipesApi.deleteRecipe(recipe.id);
    },
    onSuccess: (_, recipe) => {
      recipeDeletedToast({
        name: recipe.name,
        action: () => {
          undoDeleteRecipeMutation.mutate({ id: recipe.id });
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
