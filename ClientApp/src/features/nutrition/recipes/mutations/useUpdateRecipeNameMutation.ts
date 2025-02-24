import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { RecipeDto, RecipesApi } from "@/services/openapi";

import { setRecipesQueryData } from "../../common/queries/useRecipesQuery";
import { setRecipeQueryData } from "../queries/useRecipeQuery";

type UpdateRecipeNameMutation = {
  recipe: RecipeDto;
  name: string;
};

const recipesApi = new RecipesApi();

export default function useUpdateRecipeNameMutation() {
  const updateRecipeNameMutation = useMutation({
    mutationFn: ({ recipe, ...req }: UpdateRecipeNameMutation) => {
      return recipesApi
        .updateRecipeName(recipe.id, req)
        .then((resp) => resp.data);
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

  return { updateRecipeNameMutation, isPending };
}
