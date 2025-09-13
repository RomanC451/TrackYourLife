import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { RecipeDto, RecipesApi } from "@/services/openapi";

import { recipesQueryKeys } from "../queries/useRecipeQuery";
import useUndoDeleteRecipeMutation from "./useUndoDeleteRecipeMutation";

const recipesApi = new RecipesApi();

type Variables = {
  recipe: RecipeDto;
};

const useDeleteRecipeMutation = ({ recipeId }: { recipeId: string }) => {
  const undoDeleteRecipeMutation = useUndoDeleteRecipeMutation();

  const deleteRecipeMutation = useCustomMutation({
    mutationFn: (variables: Variables) =>
      recipesApi.deleteRecipe(variables.recipe.id),
    meta: {
      onSuccessToast: {
        type: "success",
        message: "Recipe deleted",
        data: {
          action: {
            label: "Undo",
            onClick: () => {
              undoDeleteRecipeMutation.mutate({ id: recipeId });
            },
          },
        },
      },
      invalidateQueries: [recipesQueryKeys.all],
    },
    onMutate: (variables) => {
      const previousRecipes = queryClient.getQueryData(recipesQueryKeys.all);

      if (!previousRecipes) return;

      queryClient.setQueryData(recipesQueryKeys.all, (oldData: RecipeDto[]) => [
        ...oldData.map(
          (recipe): RecipeDto =>
            recipe.id !== variables.recipe.id
              ? recipe
              : {
                  ...recipe,
                  isDeleting: true,
                },
        ),
      ]);

      return { previousRecipes };
    },

    onError: (_error, _variables, context) => {
      if (!context) return;

      queryClient.setQueryData(recipesQueryKeys.all, context.previousRecipes);
    },
  });

  return deleteRecipeMutation;
};

export default useDeleteRecipeMutation;
