import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import useStoreLoadingStateToContext from "~/hooks/useStoreLoadingStateToContext";
import { RecipeDto, RecipesApi } from "~/services/openapi";
import { setRecipeQueryData } from "../../queries/recipes/useRecipeQuery";
import { setRecipesQueryData } from "../../queries/recipes/useRecipesQuery";

type UpdateRecipeNameMutation = {
  recipe: RecipeDto;
  name: string;
};

const recipesApi = new RecipesApi();

export default function useUpdateRecipeNameMutation() {
  const updateRecipeNameMutation = useMutation({
    mutationFn: async ({ recipe, ...req }: UpdateRecipeNameMutation) => {
      recipesApi.updateRecipeName(recipe.id, req).then((resp) => resp.data);
    },
    onSuccess: (_, { recipe, name }) => {
      toast.success("Recipe name updated");
      setRecipeQueryData({ recipeId: recipe.id, name, invalidate: true });
      setRecipesQueryData({
        updatedRecipe: { ...recipe, name },
        invalidate: true,
      });
    },
    onError: () => {
      toast.error("Failed to update recipe name");
    },
  });

  const isPending = useDelayedLoading(updateRecipeNameMutation.isPending);

  useStoreLoadingStateToContext("updateRecipeNameMutation", isPending);

  return { updateRecipeNameMutation, isPending };
}
