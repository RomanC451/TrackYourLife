import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import {
  DeleteRecipesRequest,
  RecipeDto,
  RecipesApi,
} from "@/services/openapi";

import { recipesQueryKeys } from "../queries/useRecipeQuery";

const recipesApi = new RecipesApi();

const useDeleteRecipesMutation = () => {
  const deleteRecipesMutation = useCustomMutation({
    mutationFn: (request: DeleteRecipesRequest) => {
      return recipesApi.deleteRecipes(request);
    },

    meta: {
      onSuccessToast: {
        message: "Recipes deleted",
        type: "success",
      },
      invalidateQueries: [recipesQueryKeys.all],
    },

    onMutate: (request) => {
      const previousRecipes = queryClient.getQueryData(recipesQueryKeys.all);

      if (!previousRecipes) return;

      queryClient.setQueryData(recipesQueryKeys.all, (oldData: RecipeDto[]) => [
        ...oldData.map(
          (recipe): RecipeDto =>
            request.ids.includes(recipe.id)
              ? { ...recipe, isDeleting: true }
              : recipe,
        ),
      ]);

      return { previousRecipes };
    },

    onError: (_error, _variables, context) => {
      if (!context) return;

      queryClient.setQueryData(recipesQueryKeys.all, context.previousRecipes);
    },
  });

  return deleteRecipesMutation;
};

export default useDeleteRecipesMutation;
