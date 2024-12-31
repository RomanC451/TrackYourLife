import { useMutation } from "@tanstack/react-query";
import { toastDefaultServerError } from "~/data/apiSettings";
import useDelayedLoading from "~/hooks/useDelayedLoading";
import { queryClient } from "~/queryClient";
import { RecipesApi } from "~/services/openapi";
import { QUERY_KEYS } from "../../data/queryKeys";
import {
  invalidateRecipesQuery,
  setRecipesQueryData,
} from "../../queries/recipes/useRecipesQuery";
import recipeCreatedToast from "../../toasts/recipes/recipeCreatedToast";
import { createEmptyNutritionalContent } from "../../utils/nutritionalContent";

const recipesApi = new RecipesApi();

function useCreateRecipeMutation() {
  const createRecipeMutation = useMutation({
    mutationFn: (variables: { name: string }) =>
      recipesApi.createRecipe(variables).then((resp) => resp.data),
    onSuccess: (resp, variables) => {
      recipeCreatedToast(variables.name);
      setRecipesQueryData({
        newRecipe: {
          id: resp.id,
          name: variables.name,
          ingredients: [],
          nutritionalContents: createEmptyNutritionalContent(),
        },
        invalidate: true,
      });
      queryClient.setQueryData([QUERY_KEYS.recipes, resp.id], {
        id: resp.id,
        name: variables.name,
        ingredients: [],
        nutritionalContents: createEmptyNutritionalContent(),
      });

      invalidateRecipesQuery();
    },
    onError: toastDefaultServerError,
  });

  const isPending = useDelayedLoading(createRecipeMutation.isPending);

  return { createRecipeMutation, isPending };
}

export default useCreateRecipeMutation;
