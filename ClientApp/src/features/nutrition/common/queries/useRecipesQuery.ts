import { queryClient } from "@/queryClient";
import { RecipeDto } from "@/services/openapi";
import { PartialWithRequired } from "@/types/defaultTypes";

import {
  recipesQueryKeys,
  recipesQueryOptions,
} from "../../recipes/queries/useRecipeQuery";

type SetRecipesQueryDataProps = {
  data?: RecipeDto[];
  filter?: (entry: RecipeDto) => boolean;
  newRecipe?: RecipeDto;
  updatedRecipe?: PartialWithRequired<RecipeDto, "id">;
  invalidate?: boolean;
};

export const setRecipesQueryData = ({
  data,
  filter,
  newRecipe,
  updatedRecipe,
  invalidate = false,
}: SetRecipesQueryDataProps) => {
  queryClient.setQueryData(recipesQueryKeys.all, (oldData: RecipeDto[]) => {
    if (data) return data;

    let newData = oldData != undefined ? [...oldData] : [];

    if (filter) {
      newData = newData.filter(filter);
    }

    if (newRecipe) {
      newData.push(newRecipe);
    }

    if (updatedRecipe) {
      const index = newData.findIndex(
        (recipe) => recipe.id === updatedRecipe.id,
      );
      newData[index] = {
        ...newData[index],
        ...updatedRecipe,
      };
    }

    return newData;
  });

  if (invalidate) {
    queryClient.invalidateQueries(recipesQueryOptions.all);
  }
};

export function getRecipesQueryData() {
  return queryClient.getQueryData<RecipeDto[]>(recipesQueryKeys.all);
}
