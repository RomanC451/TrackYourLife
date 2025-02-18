import { useMutation } from "@tanstack/react-query";

import useDelayedLoading from "@/hooks/useDelayedLoading";
import { DeleteRecipesRequest, RecipesApi } from "@/services/openapi";
import { toastDefaultServerError } from "@/services/openapi/apiSettings";

import { setRecipesQueryData } from "../../common/queries/useRecipesQuery";
import recipesDeletedToast from "../toasts/recipesDeletedToast";

const recipesApi = new RecipesApi();

const useDeleteRecipesMutation = () => {
  const deleteRecipesMutation = useMutation({
    mutationFn: (request: DeleteRecipesRequest) => {
      return recipesApi.deleteRecipes(request);
    },
    onSuccess: (_, request) => {
      recipesDeletedToast({
        count: request.ids.length,
        action: () => {
          // TODO: Implement undo functionality
        },
      });

      setRecipesQueryData({
        filter: (entry) => !request.ids.includes(entry.id),
        invalidate: true,
      });
    },
    onError: toastDefaultServerError,
  });

  const isPending = useDelayedLoading(deleteRecipesMutation.isPending);

  return { deleteRecipesMutation, isPending };
};

export default useDeleteRecipesMutation;
