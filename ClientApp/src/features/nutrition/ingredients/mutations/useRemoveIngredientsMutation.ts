import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { IngredientDto, RecipeDto, RecipesApi } from "@/services/openapi";

import { recipesQueryKeys } from "../../recipes/queries/useRecipeQuery";

const recipesApi = new RecipesApi();

type Variables = {
  recipe: RecipeDto;
  ingredients: IngredientDto[];
};

export default function useRemoveIngredientsMutation() {
  const removeIngredientsMutation = useCustomMutation({
    mutationFn: ({ recipe, ingredients }: Variables) =>
      recipesApi
        .removeIngredients(recipe.id, {
          ingredientsIds: ingredients.map((i) => i.id),
        })
        .then((res) => res.data),

    meta: {
      onSuccessToast: {
        message: "Ingredient removed",
        type: "success",
      },
      invalidateQueries: [recipesQueryKeys.all],
    },

    onMutate: (variables) => {
      const previousData = queryClient.getQueryData<RecipeDto>(
        recipesQueryKeys.byId(variables.recipe.id),
      );

      if (!previousData) {
        return;
      }

      const idsToRemove = variables.ingredients.map((i) => i.id);

      queryClient.setQueryData(
        recipesQueryKeys.byId(variables.recipe.id),
        (oldData: RecipeDto) => {
          return structuredClone({
            ...oldData,
            ingredients: oldData.ingredients.map(
              (i): IngredientDto =>
                idsToRemove.includes(i.id) ? { ...i, isDeleting: true } : i,
            ),
          });
        },
      );

      return { previousData };
    },

    onError: (_error, variables, context) => {
      if (!context) {
        return;
      }

      queryClient.setQueryData(
        recipesQueryKeys.byId(variables.recipe.id),
        () => {
          return context.previousData;
        },
      );
    },
  });

  return removeIngredientsMutation;
}
