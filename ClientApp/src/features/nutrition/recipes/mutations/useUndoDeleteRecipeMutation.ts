import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { RecipesApi, UndoDeleteRecipeRequest } from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { setRecipesQueryData } from "../../common/queries/useRecipesQuery";

const recipesApi = new RecipesApi();

const useUndoDeleteRecipeMutation = () => {
  const undoDeleteRecipeMutation = useMutation({
    mutationFn: (request: UndoDeleteRecipeRequest) => {
      return recipesApi.undoDeleteRecipe(request);
    },
    onSuccess: (_, request) => {
      setRecipesQueryData({
        filter: (entry) => entry.id === request.id,
        invalidate: true,
      });
    },
    onError: toastDefaultServerError,
  });

  const isPending = useDelayedLoading(undoDeleteRecipeMutation.isPending);

  return { undoDeleteRecipeMutation, isPending };
};

export default useUndoDeleteRecipeMutation;
