import { queryOptions } from "@tanstack/react-query";

import { queryClient } from "@/queryClient";
import { IngredientDto, RecipeDto, RecipesApi } from "@/services/openapi";

import { QUERY_KEYS } from "../../common/data/queryKeys";
import {
  addIngredientInRecipe,
  removeIngredientFromRecipe,
  updateIngredientInRecipe,
} from "../../common/utils/recipes";

const recipesApi = new RecipesApi();

export const recipesQueryKeys = {
  all: ["recipes"] as const,
  byId: (id?: string) => [...recipesQueryKeys.all, id] as const,
};

export const recipesQueryOptions = {
  all: queryOptions({
    queryKey: recipesQueryKeys.all,
    queryFn: ({ signal }) =>
      recipesApi.getRecipesByUserId({ signal }).then((res) => res.data),
  }),
  byId: (id: string | undefined) =>
    queryOptions({
      queryKey: recipesQueryKeys.byId(id),
      queryFn: () => recipesApi.getRecipeById(id ?? "").then((res) => res.data),
      enabled: !!id,
    }),
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

      if (invalidate)
        queryClient.invalidateQueries(recipesQueryOptions.byId(recipeId));
      return newData;
    },
  );
}
