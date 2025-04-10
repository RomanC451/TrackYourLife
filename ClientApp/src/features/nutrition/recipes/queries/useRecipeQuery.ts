import { useQuery } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { queryClient } from "@/queryClient";
import { IngredientDto, RecipeDto, RecipesApi } from "@/services/openapi";

import { QUERY_KEYS } from "../../common/data/queryKeys";
import {
  addIngredientInRecipe,
  removeIngredientFromRecipe,
  updateIngredientInRecipe,
} from "../../common/utils/recipes";

const recipesApi = new RecipesApi();

function useRecipeQuery(recipeId: string) {
  const recipeQuery = useQuery({
    queryKey: [QUERY_KEYS.recipes, recipeId],
    queryFn: ({ signal }) => {
      return recipesApi
        .getRecipeById(recipeId, { signal })
        .then((res) => res.data);
    },
  });

  const isPending = useDelayedLoading(recipeQuery.isPending);

  return { recipeQuery, isPending };
}

export const prefetchRecipeQuery = (recipeId: string) => {
  queryClient.prefetchQuery({
    queryKey: [QUERY_KEYS.recipes, recipeId],
    queryFn: () => recipesApi.getRecipeById(recipeId).then((res) => res.data),
  });
};

export const invalidateRecipeQuery = (recipeId: string) => {
  queryClient.invalidateQueries({
    queryKey: [QUERY_KEYS.recipes, recipeId],
    exact: true,
  });
};

type SetRecipeQueryDataProps = {
  recipeId: string;
  data?: RecipeDto;
  name?: string;
  portions?: number;
  addedIngredient?: IngredientDto;
  updatedIngredient?: IngredientDto;
  removedIngredientsIds?: string[];
  invalidate?: boolean;
};

export function setRecipeQueryData({
  recipeId,
  data,
  name,
  portions,
  addedIngredient,
  updatedIngredient,
  removedIngredientsIds,
  invalidate = false,
}: SetRecipeQueryDataProps) {
  queryClient.setQueryData(
    [QUERY_KEYS.recipes, recipeId],
    (oldData: RecipeDto) => {
      if (data) {
        return data;
      }

      let newData = structuredClone(oldData);

      if (name) {
        newData = { ...newData, name };
      }

      if (portions) {
        newData = { ...newData, portions };
      }

      if (addedIngredient) {
        addIngredientInRecipe(newData, addedIngredient);
      }

      if (updatedIngredient) {
        newData = updateIngredientInRecipe(newData, updatedIngredient);
      }

      if (removedIngredientsIds) {
        newData = removeIngredientFromRecipe(newData, removedIngredientsIds);
      }

      if (invalidate) invalidateRecipeQuery(recipeId);
      return newData;
    },
  );
}

export function getRecipeQueryData(recipeId: string) {
  return queryClient.getQueryData<RecipeDto>([QUERY_KEYS.recipes, recipeId]);
}

export default useRecipeQuery;
