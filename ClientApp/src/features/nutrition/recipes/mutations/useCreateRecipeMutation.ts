import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { queryClient } from "@/queryClient";
import { RecipesApi } from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { QUERY_KEYS } from "../../common/data/queryKeys";
import {
  invalidateRecipesQuery,
  setRecipesQueryData,
} from "../../common/queries/useRecipesQuery";
import { createEmptyNutritionalContent } from "../../common/utils/nutritionalContent";
import recipeCreatedToast from "../toasts/recipeCreatedToast";

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
          // !! TODO: make this dynamic. add a field in the create recipe form
          portions: 1,
          isLoading: true,
          isDeleting: false,
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
