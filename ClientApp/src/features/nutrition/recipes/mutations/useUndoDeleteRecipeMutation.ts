import { useCustomMutation } from "@/hooks/useCustomMutation";
import { RecipesApi, UndoDeleteRecipeRequest } from "@/services/openapi";

import { recipesQueryKeys } from "../queries/useRecipeQuery";

const recipesApi = new RecipesApi();

type Variables = UndoDeleteRecipeRequest;

const useUndoDeleteRecipeMutation = () => {
  const undoDeleteRecipeMutation = useCustomMutation({
    mutationFn: (variables: Variables) => {
      return recipesApi.undoDeleteRecipe(variables);
    },
    meta: {
      onSuccessToast: {
        message: "Recipe undeleted",
        type: "success",
      },
      invalidateQueries: [recipesQueryKeys.all],
    },
  });

  return undoDeleteRecipeMutation;
};

export default useUndoDeleteRecipeMutation;
