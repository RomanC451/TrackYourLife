import { useMutation } from "@tanstack/react-query";
import { toast } from "sonner";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { RecipeDto, RecipesApi } from "@/services/openapi";

import { setRecipesQueryData } from "../../common/queries/useRecipesQuery";
import { setRecipeQueryData } from "../queries/useRecipeQuery";

type UpdateRecipeMutation = {
  recipe: RecipeDto;
  name: string;
  portions: number;
};

const recipesApi = new RecipesApi();

export default function useUpdateRecipeMutation() {
  const updateRecipeMutation = useMutation({
    mutationFn: ({ recipe, ...req }: UpdateRecipeMutation) => {
      return recipesApi.updateRecipe(recipe.id, req).then((resp) => resp.data);
    },
    onSuccess: (_, { recipe, name, portions }) => {
      toast.success("Recipe updated");
      setRecipeQueryData({
        recipeId: recipe.id,
        name,
        portions,
        invalidate: true,
      });
      setRecipesQueryData({
        updatedRecipe: { ...recipe, name, portions },
        invalidate: true,
      });
    },
    onError: () => {
      toast.error("Failed to update recipe name");
    },
  });

  const isPending = useDelayedLoading(updateRecipeMutation.isPending);

  return { updateRecipeMutation, isPending };
}
