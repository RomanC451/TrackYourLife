import { useQuery } from "@tanstack/react-query";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { queryClient } from "~/queryClient";
import { IngredientDto, RecipeDto, RecipesApi } from "~/services/openapi";
import { QUERY_KEYS } from "../../data/queryKeys";
import {
  addIngredientInRecipe,
  removeIngredientFromRecipe,
  updateIngredientInRecipe,
} from "../../utils/recipes";

const recipesApi = new RecipesApi();

function useRecipeQuery(recipeId: string) {
  const recipeQuery = useQuery({
    queryKey: [QUERY_KEYS.recipes, recipeId],
    queryFn: () => {
      return recipesApi.getRecipeById(recipeId).then((res) => res.data);
    },
  });

  var isPending = useDelayedLoading(recipeQuery.isPending);

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
  });
};

type SetRecipeQueryDataProps = {
  recipeId: string;
  data?: RecipeDto;
  name?: string;
  addedIngredient?: IngredientDto;
  updatedIngredient?: IngredientDto;
  removedIngredientId?: string;
  invalidate?: boolean;
};

export function setRecipeQueryData({
  recipeId,
  data,
  name,
  addedIngredient,
  updatedIngredient,
  removedIngredientId,
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
      if (addedIngredient) {
        addIngredientInRecipe(newData, addedIngredient);
      }

      if (updatedIngredient) {
        newData = updateIngredientInRecipe(newData, updatedIngredient);
      }

      if (removedIngredientId) {
        newData = removeIngredientFromRecipe(newData, removedIngredientId);
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
