import { useQuery } from "@tanstack/react-query";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { queryClient } from "~/queryClient";
import { RecipeDto, RecipesApi } from "~/services/openapi";
import { PartialWithRequired } from "~/types/defaultTypes";
import retryExcept404 from "~/utils/retry";
import { QUERY_KEYS } from "../../data/queryKeys";

const recipesApi = new RecipesApi();

function useRecipesQuery(name?: string) {
  const recipesQuery = useQuery({
    queryKey: name ? [QUERY_KEYS.recipes, name] : [QUERY_KEYS.recipes],
    queryFn: () => {
      return recipesApi.getRecipes().then((res) => res.data);
    },
    retry: retryExcept404,
  });

  const isPending = useDelayedLoading(recipesQuery.isPending);

  return { recipesQuery, isPending };
}

export const prefetchRecipesQuery = (name?: string) => {
  queryClient.prefetchQuery({
    queryKey: name ? [QUERY_KEYS.recipes, name] : [QUERY_KEYS.recipes],
    queryFn: () => recipesApi.getRecipes().then((res) => res.data),
  });
};

export const invalidateRecipesQuery = (name?: string) => {
  queryClient.invalidateQueries({
    queryKey: name ? [QUERY_KEYS.recipes, name] : [QUERY_KEYS.recipes],
  });
};

export const removeRecipesQuery = (name?: string) => {
  queryClient.removeQueries({
    queryKey: name ? [QUERY_KEYS.recipes, name] : [QUERY_KEYS.recipes],
  });
};

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
  queryClient.setQueryData([QUERY_KEYS.recipes], (oldData: RecipeDto[]) => {
    if (data) return data;

    let newData = [...oldData];

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
    invalidateRecipesQuery();
  }
};

export function getRecipesQueryData() {
  return queryClient.getQueryData<RecipeDto[]>([QUERY_KEYS.recipes]);
}

export default useRecipesQuery;
