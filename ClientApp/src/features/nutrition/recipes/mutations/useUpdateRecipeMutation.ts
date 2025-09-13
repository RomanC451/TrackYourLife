import { ErrorOption } from "react-hook-form";

import { useCustomMutation } from "@/hooks/useCustomMutation";
import { queryClient } from "@/queryClient";
import { RecipeDto, RecipesApi, UpdateRecipeRequest } from "@/services/openapi";
import { handleApiError } from "@/services/openapi/handleApiError";

import { RecipeDetailsSchema } from "../data/recipesSchemas";
import { recipesQueryKeys } from "../queries/useRecipeQuery";

type Variables = UpdateRecipeRequest;

const recipesApi = new RecipesApi();

export default function useUpdateRecipeMutation({
  recipeId,
  onSuccess,
  setError,
}: {
  recipeId: string;
  onSuccess?: () => void;
  setError: (
    name: keyof RecipeDetailsSchema,
    error: ErrorOption,
    options?: {
      shouldFocus: boolean;
    },
  ) => void;
}) {
  const updateRecipeMutation = useCustomMutation({
    mutationFn: (variables: Variables) => {
      return recipesApi
        .updateRecipe(recipeId, variables)
        .then((resp) => resp.data);
    },

    meta: {
      noDefaultErrorToast: true,
      onSuccessToast: {
        message: "Recipe updated",
        type: "success",
      },
      invalidateQueries: [recipesQueryKeys.all],
    },

    onSuccess: (_data, variables) => {
      queryClient.setQueryData(
        recipesQueryKeys.byId(recipeId),
        (oldData: RecipeDto) => ({
          ...oldData,
          name: variables.name,
          portions: variables.portions,
          isLoading: true,
        }),
      );
      onSuccess?.();
    },

    onError: (error) => {
      handleApiError({
        error,
        validationErrorsHandler: setError,
      });
    },
  });

  return updateRecipeMutation;
}
